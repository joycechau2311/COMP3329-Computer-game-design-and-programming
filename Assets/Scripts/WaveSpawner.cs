using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaveSpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject chaserPrefab;
    public GameObject flyingPrefab;

    [Header("Spawn Points")]
    public Transform leftSpawnPoint;
    public Transform rightSpawnPoint;

    [Header("Player-Relative Spawn Settings")]
    public float initialFixedSpawnX = -5f;
    public float spawnXOffsetFromPlayer = 8f;
    public float minSpawnY = 1f;
    public float maxSpawnY = 4f;
    public float spawnXRandomTweak = 0.5f;

    [Header("Debug")]
    public bool logSpawnDebug = true;

    [Header("Settings")]
    public float flyingThresholdY = 2f;
    public int requiredKills = 30;
    public string bossSceneName = "Level_4";

    private float waveTimer = 0f;
    private float nextSpawnTime = 0f;
    private float currentSpawnRate = 2.0f;
    private GameObject player;
    private Camera cachedCam;
    public float flyerSpeedMultiplier = 0.5f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        cachedCam = GetActiveCamera();

        if (IsSpawnedClone())
        {
            if (logSpawnDebug)
                UnityEngine.Debug.Log($"[SPAWN] WaveSpawner is running on spawned clone '{gameObject.name}'; disabling this instance.");
            enabled = false;
            return;
        }

        // Validate required references (removed moverPrefab check)
        if (chaserPrefab == null || flyingPrefab == null)
        {
            UnityEngine.Debug.LogWarning("[SPAWN] Missing enemy prefabs - assign Chaser and Flyer in Inspector!");
        }
    }

    bool IsSpawnedClone()
    {
        return gameObject.name.Contains("(Clone)");
    }

    void Update()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");
        if (cachedCam == null || !cachedCam.isActiveAndEnabled)
            cachedCam = GetActiveCamera();

        string currentScene = SceneManager.GetActiveScene().name;
        bool isBossScene = currentScene == bossSceneName || currentScene.Contains("Boss");

        if (isBossScene)
        {
            HandleBossSceneSpawning();
        }
        else
        {
            HandleNormalSceneSpawning();
        }
    }

    void HandleBossSceneSpawning()
    {
        if (BossHealth.bossIsDead) return;

        int currentKills = RabbitKillCounter.Instance != null ? RabbitKillCounter.Instance.GetCurrentKills() : 0;
        float bossHealthValue = 4.3f;

        GameObject boss = GameObject.Find("Boss");
        if (boss != null)
        {
            BossHealth bh = boss.GetComponent<BossHealth>();
            if (bh != null) bossHealthValue = bh.CurrentHealth;
        }

        waveTimer += Time.deltaTime;
        PhaseSettings phase = GetCurrentPhase(currentKills, bossHealthValue);
        currentSpawnRate = phase.spawnRate;

        int activeEnemies = CountLivingEnemies();

        if (activeEnemies < phase.maxEnemiesInView && Time.time >= nextSpawnTime)
        {
            SpawnEnemy(phase);
            nextSpawnTime = Time.time + currentSpawnRate;
        }
    }

    void HandleNormalSceneSpawning()
    {
        waveTimer += Time.deltaTime;

        if (waveTimer <= 15f) currentSpawnRate = 2.5f;
        else if (waveTimer <= 45f) currentSpawnRate = 1.5f;
        else if (waveTimer <= 105f) currentSpawnRate = 0.8f;
        else return;

        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemyOriginal();
            nextSpawnTime = Time.time + currentSpawnRate;
        }
    }

    void SpawnEnemyOriginal()
    {
        if (BossHealth.bossIsDead) return;

        bool canLeft = leftSpawnPoint != null;
        bool canRight = rightSpawnPoint != null;
        bool spawnLeft = canLeft && canRight ? (UnityEngine.Random.value < 0.5f) : canLeft;

        Vector3 spawnPos = GetSpawnPosition(spawnLeft);
        int sideDirection = -1;
        if (player != null)
        {
            float dx = player.transform.position.x - spawnPos.x;
            if (Mathf.Abs(dx) > 0.01f)
                sideDirection = dx > 0 ? 1 : -1;
        }

        GameObject prefabToSpawn = ChooseEnemyPrefab(); // Uses updated selection
        if (prefabToSpawn == null) return;

        // If it's a flyer, adjust height to flying threshold
        if (prefabToSpawn == flyingPrefab) spawnPos.y = flyingThresholdY;

        GameObject newEnemy = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
        SetEnemyMovementDirection(newEnemy, sideDirection);
    }

    void SpawnEnemy(PhaseSettings phase)
    {
        if (BossHealth.bossIsDead) return;

        Vector3 spawnPos = GetPlayerRelativeSpawnPosition();
        GameObject prefabToSpawn = ChooseEnemyPrefab();
        if (prefabToSpawn == null) return;

        if (prefabToSpawn == flyingPrefab) spawnPos.y = flyingThresholdY;

        GameObject newEnemy = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

        EnemyHealth enemyHealth = newEnemy.GetComponentInChildren<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.maxHealth = 1;
            enemyHealth.currentHealth = 1;
        }

        float speedToSet = phase.enemySpeed;
        if (prefabToSpawn == flyingPrefab) speedToSet *= flyerSpeedMultiplier;

        SetEnemyMovementDirection(newEnemy, -1);
        SetEnemySpeed(newEnemy, speedToSet);
    }

    Vector3 GetPlayerRelativeSpawnPosition()
    {
        if (rightSpawnPoint != null)
        {
            Vector3 p = rightSpawnPoint.position;
            p.y = UnityEngine.Random.Range(minSpawnY, maxSpawnY);
            return p;
        }
        return GetCameraBoundarySpawnPosition(spawnLeft: false);
    }

    Vector3 GetSpawnPosition(bool spawnLeft)
    {
        Transform spawnPoint = spawnLeft ? leftSpawnPoint : rightSpawnPoint;
        if (spawnPoint != null)
        {
            Vector3 p = spawnPoint.position;
            p.y = UnityEngine.Random.Range(minSpawnY, maxSpawnY);
            return p;
        }
        return GetCameraBoundarySpawnPosition(spawnLeft);
    }

    Vector3 GetCameraBoundarySpawnPosition(bool spawnLeft)
    {
        Camera cam = cachedCam != null ? cachedCam : GetActiveCamera();
        if (cam == null) return new Vector3(spawnLeft ? -8f : 8f, UnityEngine.Random.Range(minSpawnY, maxSpawnY), 0f);

        float randomSpawnY = UnityEngine.Random.Range(minSpawnY, maxSpawnY);
        Vector3 viewport = new Vector3(spawnLeft ? 0f : 1f, 0.5f, Mathf.Abs(cam.transform.position.z));
        Vector3 worldEdge = cam.ViewportToWorldPoint(viewport);

        float margin = 0.5f;
        float x = worldEdge.x + (spawnLeft ? -margin : margin);
        return new Vector3(x, randomSpawnY, 0f);
    }

    void SetEnemyMovementDirection(GameObject enemy, int direction)
    {
        if (enemy.TryGetComponent<EnemyMover>(out EnemyMover mover))
            mover.SetInitialDirection(direction);
    }

    void SetEnemySpeed(GameObject enemy, float speed)
    {
        if (enemy.TryGetComponent<EnemyMover>(out EnemyMover mover))
            mover.SetSpeed(speed);

        if (enemy.TryGetComponent<EnemyChaser>(out EnemyChaser chaser))
            chaser.SetSpeed(speed);
    }

    int CountLivingEnemies()
    {
        EnemyHealth[] allEnemies = FindObjectsOfType<EnemyHealth>(true);
        int count = 0;
        foreach (EnemyHealth enemy in allEnemies)
        {
            if (enemy == null || !enemy.gameObject.activeInHierarchy || enemy.isDead) continue;
            if (enemy.gameObject.name.Contains("(Clone)")) count++;
        }
        return count;
    }

    bool IsTemplateEnemy(EnemyHealth enemy)
    {
        if (enemy == null) return false;
        string name = enemy.gameObject.name;
        if (name.Contains("(Clone)")) return false;
        // Removed moverPrefab check[cite: 2]
        if (chaserPrefab != null && name == chaserPrefab.name) return true;
        if (flyingPrefab != null && name == flyingPrefab.name) return true;
        return false;
    }

    Camera GetActiveCamera()
    {
        if (Camera.main != null) return Camera.main;
        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        if (mainCamera != null) return mainCamera.GetComponent<Camera>();
        return null;
    }

    // New Helper: Weighted choice between Chaser and Flyer[cite: 2]
    GameObject ChooseEnemyPrefab()
    {
        if (chaserPrefab == null) return flyingPrefab;
        if (flyingPrefab == null) return chaserPrefab;

        return UnityEngine.Random.value < 0.7f ? chaserPrefab : flyingPrefab;
    }

    PhaseSettings GetCurrentPhase(int currentKills, float bossHealth)
    {
        if (currentKills < requiredKills) return new PhaseSettings(1, 5, 5f, 2f);
        if (bossHealth > 1) return new PhaseSettings(1, 10, 6f, 2f);
        return new PhaseSettings(1, 15, 7f, 2f);
    }

    struct PhaseSettings
    {
        public int minEnemiesInView;
        public int maxEnemiesInView;
        public float enemySpeed;
        public float spawnRate;

        public PhaseSettings(int min, int max, float speed, float rate)
        {
            minEnemiesInView = min;
            maxEnemiesInView = max;
            enemySpeed = speed;
            spawnRate = rate;
        }
    }
}