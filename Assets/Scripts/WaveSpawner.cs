using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaveSpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject moverPrefab;
    public GameObject chaserPrefab;
    public GameObject flyingPrefab;

    [Header("Spawn Points")]
    public Transform leftSpawnPoint;
    public Transform rightSpawnPoint;

    [Header("Player-Relative Spawn Settings")] // Core settings for your requirement
    public float initialFixedSpawnX = -5f; // Base X (left of table in Phase 1)
    public float spawnXOffsetFromPlayer = 8f; // Add this to player's X (forward movement)
    public float minSpawnY = 1f; // Min random Y
    public float maxSpawnY = 4f; // Max random Y
    public float spawnXRandomTweak = 0.5f; // Small X variation (optional)

    [Header("Debug")]
    public bool logSpawnDebug = true;

    [Header("Settings")]
    public float flyingThresholdY = 5f;
    public int requiredKills = 30;
    public string bossSceneName = "Level_4";

    private float waveTimer = 0f;
    private float nextSpawnTime = 0f;
    private float currentSpawnRate = 2.0f;
    private GameObject player; // Cache player reference

    void Start()
    {
        // Find and cache player (update in Update() to ensure it's always valid)
        player = GameObject.FindGameObjectWithTag("Player"); // Ensure player has "Player" tag

        if (IsSpawnedClone())
        {
            if (logSpawnDebug)
                UnityEngine.Debug.Log($"[SPAWN] WaveSpawner is running on spawned clone '{gameObject.name}'; disabling this instance.");
            enabled = false;
            return;
        }

        if (logSpawnDebug)
        {
            UnityEngine.Debug.Log($"[SPAWN] WaveSpawner started in scene '{SceneManager.GetActiveScene().name}'");
            UnityEngine.Debug.Log($"[SPAWN] Initial spawn X: {initialFixedSpawnX}, Player offset: {spawnXOffsetFromPlayer}");
        }

        // Validate required references
        if (moverPrefab == null || chaserPrefab == null)
        {
            UnityEngine.Debug.LogWarning("[SPAWN] Missing enemy prefabs (mover/chaser) - assign in Inspector!");
        }
    }

    bool IsSpawnedClone()
    {
        return gameObject.name.Contains("(Clone)");
    }

    void Update()
    {
        // Keep player reference updated (in case player is reloaded)
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

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

    // Handle spawning logic for Level 4 (Boss Scene)
    void HandleBossSceneSpawning()
    {
        if (BossHealth.bossIsDead)
        {
            if (logSpawnDebug)
                UnityEngine.Debug.Log("[SPAWN] Boss is dead - stopping spawns");
            return;
        }

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

        if (logSpawnDebug)
            UnityEngine.Debug.Log($"[SPAWN] Boss Phase | Kills: {currentKills} | Boss HP: {bossHealthValue} | Active Enemies: {activeEnemies}/{phase.maxEnemiesInView}");

        // HARD LIMIT - Prevent too many enemies at once
        if (activeEnemies >= 12)   // You can adjust this number
            return;

        // Spawn only if under max enemies and cooldown is done
        if (activeEnemies < phase.maxEnemiesInView && Time.time >= nextSpawnTime)
        {
            SpawnEnemy(phase);
            nextSpawnTime = Time.time + currentSpawnRate;

            if (logSpawnDebug)
                Debug.Log($"[SPAWN] Enemy spawned | Total active now: {activeEnemies + 1}");
        }
    }

    // Handle spawning for non-boss scenes (Level 1-3)
    void HandleNormalSceneSpawning()
    {
        waveTimer += Time.deltaTime;

        // Adjust spawn rate over time (original logic)
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

        // For non-boss scenes: use original spawn logic (but force left movement)
        Vector3 spawnPos = GetSpawnPosition(true); // Force spawn left (enemies move right)
        int sideDirection = -1; // Force movement to left (toward player)

        if (logSpawnDebug)
            UnityEngine.Debug.Log($"[SPAWN] Normal Scene | Spawn Pos: {spawnPos}");

        GameObject prefabToSpawn = ChooseGroundPrefab();
        if (prefabToSpawn == null)
        {
            UnityEngine.Debug.LogWarning("[SPAWN] No valid ground prefab to spawn!");
            return;
        }

        GameObject newEnemy = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
        SetEnemyMovementDirection(newEnemy, sideDirection);
    }

    void SpawnEnemy(PhaseSettings phase)
    {
        if (BossHealth.bossIsDead) return;

        // For boss scene: ONLY spawn on right (enemies move left toward player)
        Vector3 spawnPos = GetPlayerRelativeSpawnPosition();
        int sideDirection = -1; // Force movement LEFT (toward player)

        if (logSpawnDebug)
            UnityEngine.Debug.Log($"[SPAWN] Boss Scene | Spawn Pos: {spawnPos} | Movement: LEFT");

        // Choose prefab (prioritize chaser for boss scene)
        GameObject prefabToSpawn = chaserPrefab != null ? chaserPrefab : moverPrefab;
        if (prefabToSpawn == null)
        {
            UnityEngine.Debug.LogWarning("[SPAWN] No valid enemy prefab for boss scene!");
            return;
        }

        // Spawn enemy and set movement/speed
        GameObject newEnemy = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

        EnemyHealth enemyHealth = newEnemy.GetComponentInChildren<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.maxHealth = 1;
            enemyHealth.currentHealth = 1;
        }

        SetEnemyMovementDirection(newEnemy, sideDirection);
        SetEnemySpeed(newEnemy, phase.enemySpeed);
    }

    // CORE LOGIC: Calculate spawn position (follows player movement)
    Vector3 GetPlayerRelativeSpawnPosition()
    {
        // Fallback if player is missing (use initial fixed X)
        float playerX = 0f;
        if (player != null)
        {
            playerX = player.transform.position.x; // Get player's current X (forward movement)
        }
        else
        {
            UnityEngine.Debug.LogWarning("[SPAWN] Player not found - using fallback spawn X");
        }

        // Calculate final spawn X: initial fixed X + player's X + offset
        float finalSpawnX = initialFixedSpawnX + playerX + spawnXOffsetFromPlayer;

        // Add small random tweak (optional - makes spawns natural)
        finalSpawnX += UnityEngine.Random.Range(-spawnXRandomTweak, spawnXRandomTweak);

        // Random Y between min/max (your requirement)
        float randomSpawnY = UnityEngine.Random.Range(minSpawnY, maxSpawnY);

        // Return final spawn position (Z = 0 for 2D)
        return new Vector3(finalSpawnX, randomSpawnY, 0f);
    }

    // Original spawn position logic (for non-boss scenes)
    Vector3 GetSpawnPosition(bool spawnLeft)
    {
        Transform spawnPoint = spawnLeft ? leftSpawnPoint : rightSpawnPoint;
        if (spawnPoint != null)
            return spawnPoint.position;

        Camera cam = GetActiveCamera();
        if (cam != null)
        {
            float horizontalOffset = cam.orthographicSize * cam.aspect * 1.1f;
            float verticalOffset = cam.orthographicSize * 0.75f;
            Vector3 center = cam.transform.position;
            return new Vector3(center.x + (spawnLeft ? -horizontalOffset : horizontalOffset), center.y + verticalOffset, 0f);
        }

        // Final fallback
        return new Vector3(spawnLeft ? -8f : 8f, UnityEngine.Random.Range(minSpawnY, maxSpawnY), 0f);
    }

    // Helper: Set enemy movement direction (force left for boss scene)
    void SetEnemyMovementDirection(GameObject enemy, int direction)
    {
        if (enemy.TryGetComponent<EnemyMover>(out EnemyMover mover))
        {
            mover.SetInitialDirection(direction);
        }
    }

    // Helper: Set enemy speed (for boss scene phases)
    void SetEnemySpeed(GameObject enemy, float speed)
    {
        if (enemy.TryGetComponent<EnemyMover>(out EnemyMover mover))
        {
            mover.SetSpeed(speed);
        }

        if (enemy.TryGetComponent<EnemyChaser>(out EnemyChaser chaser))
        {
            chaser.SetSpeed(speed);
        }
    }

    // IMPROVED CountLivingEnemies - This was the main culprit
    int CountLivingEnemies()
    {
        EnemyHealth[] allEnemies = FindObjectsOfType<EnemyHealth>(true);

        int count = 0;
        foreach (EnemyHealth enemy in allEnemies)
        {
            if (enemy == null) continue;
            if (!enemy.gameObject.activeInHierarchy) continue;
            if (enemy.isDead) continue;

            // Only count real spawned enemies (clones), not prefabs
            if (enemy.gameObject.name.Contains("(Clone)"))
            {
                count++;
            }
        }
        return count;
    }

    // Helper: Check if enemy is a template (not a spawned clone)
    bool IsTemplateEnemy(EnemyHealth enemy)
    {
        if (enemy == null) return false;

        string name = enemy.gameObject.name;
        if (name.Contains("(Clone)")) return false;

        if (moverPrefab != null && name == moverPrefab.name) return true;
        if (chaserPrefab != null && name == chaserPrefab.name) return true;
        if (flyingPrefab != null && name == flyingPrefab.name) return true;

        return false;
    }

    // Helper: Get active camera (original logic)
    Camera GetActiveCamera()
    {
        if (Camera.main != null) return Camera.main;

        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        if (mainCamera != null) return mainCamera.GetComponent<Camera>();

        foreach (Camera cam in Camera.allCameras)
        {
            if (cam.isActiveAndEnabled) return cam;
        }

        return null;
    }

    // Helper: Choose ground prefab (original logic)
    GameObject ChooseGroundPrefab()
    {
        if (moverPrefab != null && chaserPrefab != null)
            return UnityEngine.Random.value > 0.5f ? moverPrefab : chaserPrefab;

        return chaserPrefab ?? moverPrefab;
    }

    // Helper: Get current boss phase (original logic)
    PhaseSettings GetCurrentPhase(int currentKills, float bossHealth)
    {
        if (currentKills < requiredKills) return new PhaseSettings(1, 5, 5f, 5.0f);
        if (bossHealth > 1) return new PhaseSettings(1, 10, 6f, 5.0f);
        return new PhaseSettings(1, 15, 7f, 5.0f);
    }

    // Phase settings struct (original logic)
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

    //public void StopSpawning()
    //{
    //    if (bossHealth._isDead != null)
    //        StopCoroutine(spawnCoroutine);
    //}spawnCoroutine
}