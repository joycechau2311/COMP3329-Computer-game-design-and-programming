using System.Collections;
using UnityEngine;

public class Level2Defender : MonoBehaviour
{
    private UIManager ui;

    [Header("Core References")]
    public GameObject tony;
    public TonyHealth tonyHealth;
    public GameObject teleportGate;          

    [Header("Wave Settings")]
    public float totalWaveTime = 120f;
    public int requiredPotionProgress = 100;

    public float GetPotionProgress() => potionProgress;
    public float GetMaxPotion() => requiredPotionProgress;

    WaveSettings GetCurrentWave(float currentTime)
    {
        if (currentTime < 20f)       
            return new WaveSettings(2f, 1.0f);  

        if (currentTime < 80f)    
            return new WaveSettings(1.5f, 1.4f);   // Faster spawn, a bit faster enemies

        return new WaveSettings(2f, 1.8f);  
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
        ui = FindObjectOfType<UIManager>();
    }

    private void Update()
    {
        if (ui != null)
            ui.UpdatePotionBar(potionProgress, requiredPotionProgress);
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
        Camera cam = Camera.main;
        float y = Random.Range(-2f, 2f); // adjust to your ground height

        float x;
        if (Random.value < 0.5f)
            x = cam.transform.position.x - cam.orthographicSize * cam.aspect - 1f; // left
        else
            x = cam.transform.position.x + cam.orthographicSize * cam.aspect + 1f; // right

        Vector3 spawnPos = new Vector3(x, y, 0f);

        GameObject prefab = ChooseEnemyPrefab();
        Instantiate(prefab, spawnPos, Quaternion.identity);
    }

    GameObject ChooseEnemyPrefab()
    {
        float roll = Random.value;

        if (roll < 0.4f)
            return moverPrefab;
        else if (roll < 0.8f)
            return chaserPrefab;
        else
            return flyerPrefabs;
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

    Vector2 GetCameraBounds()
    {
        Camera cam = Camera.main;
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;

        Vector3 camPos = cam.transform.position;
        return new Vector2(width / 2f, height / 2f);
    }

    // For testing
    [ContextMenu("Force Show Gate")]
    public void DebugShowGate()
    {
        ShowTeleportGate();
    }
}