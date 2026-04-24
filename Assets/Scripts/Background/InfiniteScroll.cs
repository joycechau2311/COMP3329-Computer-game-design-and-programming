using System;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteScroll : MonoBehaviour
{
    [Header("Core Settings")]
    public Transform player;
    [Tooltip("Exact width of your BG tile (match sprite width ÷ PPU)")]
    public float tileWidth = 19.2f;
    public float spawnAheadDistance = 20f;

    [Header("Scene References")]
    public SpriteRenderer bgTemplate;      // Drag BG SpriteRenderer from Hierarchy
    public GameObject groundTemplate;      // Drag Ground GameObject (with BoxCollider2D) from Hierarchy
    public SpriteRenderer boxTemplate;     // Drag Box SpriteRenderer from Hierarchy (optional)

    [Header("Box Obstacles")]
    public Vector2 boxSpawnRangeX = new Vector2(5f, 9f);
    [Range(0f, 0.3f)] public float boxSpawnChance = 0.2f;
    public float boxYPosition = -1.5f;

    [Header("Layer Settings")]
    public int boxLayer = 8;

    private bool flipNextBg = true;
    private List<GameObject> bgList = new List<GameObject>();
    private List<GameObject> groundList = new List<GameObject>();
    private List<GameObject> boxList = new List<GameObject>();

    private float lastSpawnX;
    private float firstSpawnX;
    private float boxWidth;

    // Store original Y positions
    private float bgY;
    private float groundY;
    private float boxZ;

    private void Start()
    {
        // Validate references
        if (player == null || bgTemplate == null || groundTemplate == null)
        {
            UnityEngine.Debug.LogError("Assign Player, BG Template, Ground Template in Inspector!");
            enabled = false;
            return;
        }

        // Store original Y positions
        bgY = bgTemplate.transform.position.y;
        groundY = groundTemplate.transform.position.y;
        boxZ = boxTemplate != null ? boxTemplate.transform.position.z : 0;

        // Auto-detect BG width (including scale)
        tileWidth = bgTemplate.bounds.size.x;

        // Get box width
        if (boxTemplate != null)
        {
            boxWidth = boxTemplate.bounds.size.x;
        }

        // Initialize spawn position based on player tile index
        firstSpawnX = Mathf.Floor(player.position.x / tileWidth) * tileWidth;
        lastSpawnX = firstSpawnX;

        // Pre-spawn tiles around player
        for (int i = -1; i <= 2; i++)
        {
            SpawnTile(firstSpawnX + i * tileWidth);
        }
    }

    private void Update()
    {
        if (player == null)
        {
            enabled = false;
            return;
        }

        // Calculate current tile index
        float playerTileIndex = Mathf.Floor(player.position.x / tileWidth);

        // Spawn tiles ahead of player
        float rightSpawnIndex = playerTileIndex + 2;
        for (float i = Mathf.Ceil(lastSpawnX / tileWidth); i <= rightSpawnIndex; i++)
        {
            float spawnX = i * tileWidth;
            if (!IsTileAlreadySpawned(spawnX))
            {
                lastSpawnX = spawnX;
                SpawnTile(spawnX);
            }
        }

        // Spawn tiles behind player (for left movement)
        float leftSpawnIndex = playerTileIndex - 1;
        if (lastSpawnX > player.position.x + spawnAheadDistance)
        {
            float newLastSpawnX = leftSpawnIndex * tileWidth;
            if (newLastSpawnX < lastSpawnX - tileWidth)
            {
                lastSpawnX = newLastSpawnX;
                SpawnTile(lastSpawnX);
            }
        }

        CleanBehind();
    }

    private bool IsTileAlreadySpawned(float spawnX)
    {
        foreach (var bg in bgList)
        {
            if (bg != null && Mathf.Abs(bg.transform.position.x - spawnX) < 0.1f)
                return true;
        }
        return false;
    }

    private void SpawnTile(float spawnX)
    {
        // Spawn BG - create new GameObject with SpriteRenderer (no Instantiate needed)
        GameObject newBg = new GameObject("BG_" + Mathf.Round(spawnX));
        SpriteRenderer bgSr = newBg.AddComponent<SpriteRenderer>();
        bgSr.sprite = bgTemplate.sprite;
        bgSr.color = bgTemplate.color;
        bgSr.sortingLayerID = bgTemplate.sortingLayerID;
        bgSr.sortingOrder = bgTemplate.sortingOrder;
        
        newBg.transform.position = new Vector3(
            Mathf.Round(spawnX * 100f) / 100f,
            bgY,
            bgTemplate.transform.position.z
        );
        newBg.transform.localScale = bgTemplate.transform.localScale;

        // Flip X based on tile index for seamless pattern
        int tileIndex = Mathf.RoundToInt(spawnX / tileWidth);
        Vector3 scale = bgTemplate.transform.localScale;
        if (tileIndex % 2 == 1)
            scale.x = -Mathf.Abs(scale.x);
        else
            scale.x = Mathf.Abs(scale.x);
        newBg.transform.localScale = scale;

        newBg.transform.parent = transform;
        bgList.Add(newBg);

        // Spawn Ground (collider only, invisible)
        GameObject newGround = new GameObject("Ground_" + Mathf.Round(spawnX));
        // Copy BoxCollider2D from template
        BoxCollider2D templateCollider = groundTemplate.GetComponent<BoxCollider2D>();
        if (templateCollider != null)
        {
            BoxCollider2D newCollider = newGround.AddComponent<BoxCollider2D>();
            // Copy collider settings
            newCollider.size = templateCollider.size;
            newCollider.offset = templateCollider.offset;
            newCollider.isTrigger = templateCollider.isTrigger;
            newCollider.usedByComposite = templateCollider.usedByComposite;
            newCollider.edgeRadius = templateCollider.edgeRadius;
        }
        newGround.transform.position = new Vector3(
            Mathf.Round(spawnX * 100f) / 100f,
            groundY,
            groundTemplate.transform.position.z
        );
        newGround.transform.localScale = groundTemplate.transform.localScale;
        newGround.transform.parent = transform;
        groundList.Add(newGround);

        // Spawn Boxes
        if (boxTemplate != null)
        {
            SpawnBoxes(spawnX);
        }
    }

    private void SpawnBoxes(float tileX)
    {
        if (boxTemplate == null) return;

        // Get box width from collider (more accurate for collision)
        BoxCollider2D boxCollider = boxTemplate.GetComponent<BoxCollider2D>();
        float actualBoxWidth = boxCollider != null ? boxCollider.size.x * Mathf.Abs(boxTemplate.transform.localScale.x) : boxTemplate.bounds.size.x;
        if (actualBoxWidth <= 0) return;

        float minGap = Mathf.Max(0.5f, boxSpawnRangeX.x); // never less than 0.5 units
        float maxGap = Mathf.Max(minGap, boxSpawnRangeX.y);

        float currentX = tileX - tileWidth / 2 + actualBoxWidth / 2 + minGap;
        float endX = tileX + tileWidth / 2 - actualBoxWidth / 2 - minGap;

        while (currentX < endX)
        {
            if (UnityEngine.Random.value <= boxSpawnChance)
            {
                GameObject newBox = new GameObject("Box_" + Mathf.Round(currentX));
                SpriteRenderer boxSr = newBox.AddComponent<SpriteRenderer>();
                boxSr.sprite = boxTemplate.sprite;
                boxSr.color = boxTemplate.color;
                boxSr.sortingLayerID = boxTemplate.sortingLayerID;
                boxSr.sortingOrder = 1; // Boxes in front

                newBox.transform.position = new Vector3(
                    Mathf.Round(currentX * 100f) / 100f,
                    boxYPosition,
                    boxZ
                );
                newBox.transform.localScale = boxTemplate.transform.localScale;
                newBox.transform.parent = transform;

                // Add BoxCollider2D for collision
                if (boxCollider != null)
                {
                    BoxCollider2D newCollider = newBox.AddComponent<BoxCollider2D>();
                    newCollider.size = boxCollider.size;
                    newCollider.offset = boxCollider.offset;
                    newCollider.isTrigger = boxCollider.isTrigger;
                }

                boxList.Add(newBox);

                // Always move by collider width + at least min gap (never overlap)
                float gap = UnityEngine.Random.Range(minGap, maxGap);
                currentX += actualBoxWidth + gap;
            }
            else
            {
                // Skip ahead by at least min gap
                float gap = UnityEngine.Random.Range(minGap, maxGap);
                currentX += gap;
            }
        }
    }

    private void CleanBehind()
    {
        if (player == null) return;

        float cleanMinX = player.position.x - spawnAheadDistance * 2;
        float cleanMaxX = player.position.x + spawnAheadDistance * 2;

        CleanList(bgList, cleanMinX, cleanMaxX);
        CleanList(groundList, cleanMinX, cleanMaxX);
        CleanList(boxList, cleanMinX, cleanMaxX);
    }

    private void CleanList(List<GameObject> list, float minX, float maxX)
    {
        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (list[i] == null)
            {
                list.RemoveAt(i);
                continue;
            }

            float x = list[i].transform.position.x;
            if (x < minX || x > maxX)
            {
                Destroy(list[i]);
                list.RemoveAt(i);
            }
        }
    }

    public void ResetGenerator()
    {
        foreach (var o in bgList) if (o != null) Destroy(o);
        foreach (var o in groundList) if (o != null) Destroy(o);
        foreach (var o in boxList) if (o != null) Destroy(o);

        bgList.Clear();
        groundList.Clear();
        boxList.Clear();

        flipNextBg = true;

        if (player != null)
        {
            firstSpawnX = Mathf.Floor(player.position.x / tileWidth) * tileWidth;
            lastSpawnX = firstSpawnX;

            for (int i = -1; i <= 2; i++)
            {
                SpawnTile(firstSpawnX + i * tileWidth);
            }
        }
    }
}