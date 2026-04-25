using System.Collections.Generic;
using UnityEngine;

public class InfiniteScroll : MonoBehaviour
{
    [Header("Core Settings")]
    public Transform player;
    public float spawnAheadDistance = 20f;

    [Header("Templates (use scene objects as templates)")]
    public SpriteRenderer bgTemplate;   // in-scene BG tile used as template
    public GameObject groundTemplate;   // in-scene Ground tile used as template (sprite + collider)
    public GameObject boxTemplate;      // optional in-scene Box used as template (sprite + collider)

    [Header("Boxes")]
    [Range(0f, 1f)] public float boxSpawnChancePerTile = 0.35f;
    [Tooltip("If a tile is chosen to spawn boxes, at least this many will be created.")]
    public int minBoxesPerChosenTile = 1;
    public Vector2 boxCountRange = new Vector2(0, 2); // inclusive-ish (we'll round)
    [Tooltip("If enabled, boxes will be placed on top of the ground collider/sprite automatically.")]
    public bool placeBoxesOnGroundTop = true;
    public float boxYPosition = -1.5f;
    [Tooltip("Optional tag to apply to spawned boxes (leave empty to keep template tag).")]
    public string boxTag = "Obstacle";
    [Tooltip("Avoid spawning a box on top of player/enemies (prevents movers getting embedded and flip-spamming).")]
    public bool avoidSpawningBoxesOnCharacters = true;
    public float avoidCharacterRadius = 1.2f;
    [Tooltip("If enabled, enforce low-friction colliders on spawned boxes to reduce 'sticking'.")]
    public bool forceLowFrictionBoxes = true;

    [Header("Layers")]
    [Tooltip("Layer assigned to spawned boxes so EnemyMover can rebound via obstacleMask. Prefer setting by name for portability.")]
    public string boxLayerName = "Obstacle";
    [Tooltip("Fallback layer index if boxLayerName is not found.")]
    public int boxLayer = 15;

    private float tileWidth;
    private float baseX;
    private float cachedGroundTopY;
    private float cachedBoxHalfHeight;
    private float cachedBoxHalfWidth;
    private PhysicsMaterial2D lowFrictionMaterial;

    private readonly Dictionary<int, GameObject> bgByIndex = new();
    private readonly Dictionary<int, GameObject> groundByIndex = new();
    private readonly Dictionary<int, List<GameObject>> boxesByIndex = new();
    private bool warnedMissingBoxTemplate;

    private void Start()
    {
        if (player == null || bgTemplate == null || groundTemplate == null)
        {
            Debug.LogError("Assign Player, BG Template, Ground Template in Inspector!");
            enabled = false;
            return;
        }

        tileWidth = bgTemplate.bounds.size.x;
        if (tileWidth <= 0.001f)
        {
            Debug.LogError("BG Template bounds width is invalid.");
            enabled = false;
            return;
        }

        baseX = bgTemplate.transform.position.x;

        CacheBoxAndGroundHeights();

        // Hide templates (keep them in scene as references/prefabs)
        bgTemplate.gameObject.SetActive(false);
        groundTemplate.SetActive(false);
        if (boxTemplate != null) boxTemplate.SetActive(false);
        else Debug.LogWarning("Box Template is not assigned; boxes will not be generated.");

        // Spawn initial band of tiles
        int centerIndex = WorldXToIndex(player.position.x);
        EnsureTiles(centerIndex);
    }

    private void CacheBoxAndGroundHeights()
    {
        // Ground top (prefer collider; fallback to sprite bounds)
        cachedGroundTopY = groundTemplate.transform.position.y;
        var gc = groundTemplate.GetComponent<Collider2D>();
        if (gc != null)
            cachedGroundTopY = gc.bounds.max.y;
        else
        {
            var gsr = groundTemplate.GetComponent<SpriteRenderer>();
            if (gsr != null) cachedGroundTopY = gsr.bounds.max.y;
        }

        // Box half height (prefer collider; fallback to sprite bounds)
        cachedBoxHalfHeight = 0.5f;
        cachedBoxHalfWidth = 0.5f;
        if (boxTemplate != null)
        {
            var bc = boxTemplate.GetComponent<Collider2D>();
            if (bc != null)
            {
                cachedBoxHalfHeight = bc.bounds.extents.y;
                cachedBoxHalfWidth = bc.bounds.extents.x;
            }
            else
            {
                var bsr = boxTemplate.GetComponent<SpriteRenderer>();
                if (bsr != null)
                {
                    cachedBoxHalfHeight = bsr.bounds.extents.y;
                    cachedBoxHalfWidth = bsr.bounds.extents.x;
                }
            }
        }

        if (forceLowFrictionBoxes && lowFrictionMaterial == null)
        {
            lowFrictionMaterial = new PhysicsMaterial2D("InfiniteScroll_LowFriction")
            {
                friction = 0f,
                bounciness = 0f
            };
        }
    }

    private void Update()
    {
        if (player == null) return;

        if (!warnedMissingBoxTemplate && boxTemplate == null)
        {
            warnedMissingBoxTemplate = true;
            Debug.LogWarning("InfiniteScroll: Box Template is not assigned, so boxes cannot be generated.");
        }

        int centerIndex = WorldXToIndex(player.position.x);
        EnsureTiles(centerIndex);
        CleanupFar(centerIndex);
    }

    private int WorldXToIndex(float worldX)
    {
        return Mathf.RoundToInt((worldX - baseX) / tileWidth);
    }

    private float IndexToWorldX(int index)
    {
        return baseX + index * tileWidth;
    }

    private void EnsureTiles(int centerIndex)
    {
        int range = Mathf.CeilToInt(spawnAheadDistance / tileWidth) + 2;
        int min = centerIndex - range;
        int max = centerIndex + range;

        for (int i = min; i <= max; i++)
        {
            SpawnTileIfMissing(i);
        }
    }

    private void SpawnTileIfMissing(int index)
    {
        float x = IndexToWorldX(index);
        bool flip = (Mathf.Abs(index) % 2) == 1; // first tile (index 0) not flipped, next flipped, ...

        if (!bgByIndex.ContainsKey(index))
        {
            GameObject bg = Instantiate(bgTemplate.gameObject, transform);
            bg.name = $"BG_{index}";
            bg.SetActive(true);
            bg.transform.position = new Vector3(x, bgTemplate.transform.position.y, bgTemplate.transform.position.z);

            SpriteRenderer sr = bg.GetComponent<SpriteRenderer>();
            if (sr != null) sr.flipX = flip;

            bgByIndex[index] = bg;
        }

        if (!groundByIndex.ContainsKey(index))
        {
            GameObject ground = Instantiate(groundTemplate, transform);
            ground.name = $"Ground_{index}";
            ground.SetActive(true);
            ground.transform.position = new Vector3(x, groundTemplate.transform.position.y, groundTemplate.transform.position.z);

            SpriteRenderer gsr = ground.GetComponent<SpriteRenderer>();
            if (gsr != null) gsr.flipX = flip;

            groundByIndex[index] = ground;
        }

        if (boxTemplate != null && !boxesByIndex.ContainsKey(index))
        {
            if (Random.value > boxSpawnChancePerTile)
                return; // don't record this tile; allow retry if tile is revisited

            boxesByIndex[index] = new List<GameObject>();
            int count = Mathf.RoundToInt(Random.Range(boxCountRange.x, boxCountRange.y));
            count = Mathf.Clamp(count, 0, 10);
            count = Mathf.Max(minBoxesPerChosenTile, count);

            for (int k = 0; k < count; k++)
            {
                // spawn within this tile bounds
                float half = tileWidth * 0.5f;
                float localX = Random.Range(-half * 0.75f, half * 0.75f);
                float bx = x + localX;

                GameObject box = Instantiate(boxTemplate, transform);
                box.name = $"Box_{index}_{k}";
                box.SetActive(true);
                ApplyObstacleIdentity(box);
                if (!string.IsNullOrEmpty(boxTag))
                {
                    // Only assign if tag exists in project; otherwise Unity will throw.
                    try { box.tag = boxTag; } catch { /* ignore */ }
                }
                // Use Z=0 by default for 2D visibility unless the template has a specific Z you need.
                float bz = Mathf.Approximately(boxTemplate.transform.position.z, 0f) ? 0f : boxTemplate.transform.position.z;
                float by = placeBoxesOnGroundTop ? (cachedGroundTopY + cachedBoxHalfHeight) : boxYPosition;
                Vector3 spawnPos = new Vector3(bx, by, bz);

                if (avoidSpawningBoxesOnCharacters && WouldOverlapCharacter(spawnPos))
                    continue;

                box.transform.position = spawnPos;

                ConfigureSpawnedBoxPhysics(box);

                boxesByIndex[index].Add(box);
            }
        }
    }

    private void ApplyObstacleIdentity(GameObject box)
    {
        int layerToUse = boxLayer;
        if (!string.IsNullOrWhiteSpace(boxLayerName))
        {
            int named = LayerMask.NameToLayer(boxLayerName);
            if (named != -1) layerToUse = named;
        }

        // Apply to whole hierarchy so any child collider also counts as obstacle
        foreach (Transform t in box.GetComponentsInChildren<Transform>(true))
            t.gameObject.layer = layerToUse;
    }

    private bool WouldOverlapCharacter(Vector3 boxPos)
    {
        // Quick check: avoid spawning right on the player or any enemy-like object.
        Collider2D[] hits = Physics2D.OverlapCircleAll(boxPos, avoidCharacterRadius);
        foreach (var h in hits)
        {
            if (h == null) continue;
            if (h.isTrigger) continue;

            if (h.CompareTag("Player"))
                return true;

            // Generic detection without requiring teammates to share exact tags/layers:
            // any object with "Enemy" in its name or an EnemyHealth component counts.
            if (h.GetComponentInParent<EnemyHealth>() != null)
                return true;

            string n = h.gameObject.name;
            if (!string.IsNullOrEmpty(n) && n.ToLowerInvariant().Contains("enemy"))
                return true;
        }
        return false;
    }

    private void ConfigureSpawnedBoxPhysics(GameObject box)
    {
        // Ensure boxes are static colliders (so movers rebound cleanly and boxes don't jitter).
        Rigidbody2D rb2d = box.GetComponent<Rigidbody2D>();
        if (rb2d == null)
            rb2d = box.AddComponent<Rigidbody2D>();
        rb2d.bodyType = RigidbodyType2D.Static;
        rb2d.simulated = true;

        // Ensure collider exists and is non-trigger; apply low friction if requested.
        Collider2D col = box.GetComponent<Collider2D>();
        if (col == null)
            col = box.AddComponent<BoxCollider2D>();
        col.isTrigger = false;

        if (forceLowFrictionBoxes && lowFrictionMaterial != null)
            col.sharedMaterial = lowFrictionMaterial;
    }

    private void CleanupFar(int centerIndex)
    {
        int range = Mathf.CeilToInt(spawnAheadDistance / tileWidth) + 4;
        int minKeep = centerIndex - range;
        int maxKeep = centerIndex + range;

        CleanupDict(bgByIndex, minKeep, maxKeep);
        CleanupDict(groundByIndex, minKeep, maxKeep);

        // Boxes
        var keys = new List<int>(boxesByIndex.Keys);
        foreach (int idx in keys)
        {
            if (idx >= minKeep && idx <= maxKeep) continue;
            foreach (var b in boxesByIndex[idx])
                if (b != null) Destroy(b);
            boxesByIndex.Remove(idx);
        }
    }

    private void CleanupDict(Dictionary<int, GameObject> dict, int minKeep, int maxKeep)
    {
        var keys = new List<int>(dict.Keys);
        foreach (int idx in keys)
        {
            if (idx >= minKeep && idx <= maxKeep) continue;
            if (dict[idx] != null) Destroy(dict[idx]);
            dict.Remove(idx);
        }
    }

    public void ResetGenerator()
    {
        foreach (var kv in bgByIndex) if (kv.Value != null) Destroy(kv.Value);
        foreach (var kv in groundByIndex) if (kv.Value != null) Destroy(kv.Value);
        foreach (var kv in boxesByIndex)
            foreach (var b in kv.Value) if (b != null) Destroy(b);

        bgByIndex.Clear();
        groundByIndex.Clear();
        boxesByIndex.Clear();

        if (player != null)
        {
            int centerIndex = WorldXToIndex(player.position.x);
            EnsureTiles(centerIndex);
        }
    }
}