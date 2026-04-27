using UnityEngine;

public class BossManager : MonoBehaviour
{
    public static BossManager Instance;

    [Header("Boss")]
    public BossHealth bossHealth;

    [Header("References")]
    public InfiniteScroll infiniteScroll;
    public UIManager uiManager;

    private Camera mainCam;
    public bool bossFightStarted = false;
    public bool IsBossFightActive => bossFightStarted;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        mainCam = Camera.main;
        FollowPlayerXPosition follow = bossHealth.GetComponent<FollowPlayerXPosition>();
        if (follow != null)
            follow.enabled = false;
    }

    private void Update()
    {
        if (bossFightStarted) return;
        if (bossHealth == null || mainCam == null) return;

        // Check if boss is in camera vision
        Vector3 viewportPos = mainCam.WorldToViewportPoint(bossHealth.transform.position);

        bool bossVisible =
            viewportPos.z > 0f &&
            viewportPos.x > 0f && viewportPos.x < 1f &&
            viewportPos.y > 0f && viewportPos.y < 1f;

        if (bossVisible)
        {
            TriggerBossFight();
        }
    }

    private void TriggerBossFight()
    {
        if (bossFightStarted) return;
        bossFightStarted = true;

        Debug.Log("👑 Boss entered vision — fight started!");

        // Stop infinite scrolling
        if (infiniteScroll != null)
            infiniteScroll.enabled = false;

        // Enable boss follow AFTER reveal
        FollowPlayerXPosition follow = bossHealth.GetComponent<FollowPlayerXPosition>();
        if (follow != null)
            follow.enabled = true;

        // ✅ Subscribe UI BEFORE activation
        bossHealth.OnHealthChanged += uiManager.UpdateBossHealth;
        bossHealth.OnBossDied += EndBossFight;

        // Show boss UI now (but no values yet)
        uiManager.ShowBossUI(true);

        // Activate boss fight (fires initial UI update)
        bossHealth.ActivateBossFight();

        // Enable combat systems
        EnableIfExists<BossPhaseController>();
        EnableIfExists<BossPhase2to3>();
        EnableIfExists<BossHitAnimation>();
        EnableIfExists<BossFloatAndFreeze>();

        Collider2D col = bossHealth.GetComponent<Collider2D>();
        if (col != null)
            col.enabled = true;
    }


    private void EndBossFight()
    {
        Debug.Log("✅ Boss fight ended");

        uiManager.ShowBossUI(false);

        bossHealth.OnHealthChanged -= uiManager.UpdateBossHealth;
        bossHealth.OnBossDied -= EndBossFight;
    }

    private void EnableIfExists<T>() where T : Behaviour
    {
        T comp = bossHealth.GetComponent<T>();
        if (comp != null)
        {
            comp.enabled = true;
            Debug.Log($"✅ Enabled {typeof(T).Name}");
        }
    }
}