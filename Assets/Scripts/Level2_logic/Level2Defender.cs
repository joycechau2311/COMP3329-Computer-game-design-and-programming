using System.Collections;
using UnityEngine;

public class Level2Defender : MonoBehaviour
{
    [Header("Core References")]
    public GameObject tony;
    public TonyHealth tonyHealth;
    public GameObject teleportGate;           // ← Your hidden gate

    [Header("Wave Settings")]
    public float totalWaveTime = 120f;
    public int requiredPotionProgress = 100;

    WaveSettings GetCurrentWave(float currentTime)
    {
        if (currentTime < 40f)       // First 40 seconds
            return new WaveSettings(2.5f, 1.0f);   // Slow spawn (every 2.5s), normal speed

        if (currentTime < 80f)       // From 40s to 80s
            return new WaveSettings(1.6f, 1.4f);   // Faster spawn, a bit faster enemies

        return new WaveSettings(1.1f, 1.8f);       // After 80s → very fast and aggressive
    }

    [Header("Enemy Prefabs")]
    public GameObject moverPrefab;
    public GameObject chaserPrefab;
    public GameObject flyerPrefabs;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    private float timer = 0f;
    private float potionProgress = 0f;
    private Coroutine waveCoroutine;
    private bool levelComplete = false;
    private bool gateShown = false;

    void Start()
    {
        // Make sure gate is hidden at start
        if (teleportGate != null)
            teleportGate.SetActive(false);

        if (tonyHealth == null && tony != null)
            tonyHealth = tony.GetComponent<TonyHealth>();

        StartLevel();
    }

    public void StartLevel()
    {
        timer = 0f;
        potionProgress = 0f;
        levelComplete = false;
        gateShown = false;

        waveCoroutine = StartCoroutine(WaveSystemRoutine());
        StartCoroutine(PotionBrewingRoutine());

        Debug.Log("=== Level 2 Started - Protect Tony until potion is ready ===");
    }

    IEnumerator WaveSystemRoutine()
    {
        while (timer < totalWaveTime && !levelComplete)
        {
            timer += Time.deltaTime;

            WaveSettings currentWave = GetCurrentWave(timer);

            if (Time.time % currentWave.spawnInterval < Time.deltaTime)
            {
                SpawnEnemy(currentWave);
            }

            yield return null;
        }

        CheckWinCondition();
    }

    IEnumerator PotionBrewingRoutine()
    {
        while (potionProgress < requiredPotionProgress && !levelComplete)
        {
            float brewSpeed = 6f;

            if (tony != null)
            {
                float distance = Vector2.Distance(tony.transform.position, GameObject.FindGameObjectWithTag("Player").transform.position);
                brewSpeed = (distance < 7f) ? 10f : 6f;
            }

            potionProgress += brewSpeed * Time.deltaTime;
            potionProgress = Mathf.Clamp(potionProgress, 0f, requiredPotionProgress);

            yield return null;
        }

        CheckWinCondition();
    }

    void CheckWinCondition()
    {
        if (levelComplete) return;
        if (gateShown) return;

        levelComplete = true;

        if (potionProgress >= requiredPotionProgress && tonyHealth != null && !tonyHealth.isDead)
        {
            ShowTeleportGate();
        }
        else
        {
            Debug.Log("Level 2 Failed");
        }
    }

    private void ShowTeleportGate()
    {
        gateShown = true;

        if (teleportGate != null)
        {
            teleportGate.SetActive(true);
            Debug.Log("✅ Teleport Gate has appeared! Level 2 Complete.");
        }

        if (tonyHealth != null)
            tonyHealth.PlayWinAnimation();
    }

    void SpawnEnemy(WaveSettings wave)
    {
        if (spawnPoints.Length == 0) return;

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject prefab = (Random.value > 0.5f) ? chaserPrefab : moverPrefab;

        if (prefab != null)
            Instantiate(prefab, spawnPoint.position, Quaternion.identity);
    }

    struct WaveSettings
    {
        public float spawnInterval;
        public float enemySpeed;

        public WaveSettings(float interval, float speed)
        {
            spawnInterval = interval;
            enemySpeed = speed;
        }
    }

    // For testing
    [ContextMenu("Force Show Gate")]
    public void DebugShowGate()
    {
        ShowTeleportGate();
    }
}