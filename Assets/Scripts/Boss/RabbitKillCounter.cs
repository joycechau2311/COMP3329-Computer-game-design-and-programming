using UnityEngine;

public class RabbitKillCounter : MonoBehaviour
{
    [Header("Kill Settings")]
    public int requiredKills = 30;
    public string rabbitTag = "Enemy";
    public string bulletTag = "Bullet";

    [Header("References")]
    public GameObject tableObject;
    public Animator bossAnimator;
    public string tableExplodeAnim = "Table_Bloom";
    public float tableDestroyDelay = 0.5f;

    public int _currentKills = 0;
    private bool _hasTriggered = false;
    private bool _phase2Triggered = false;

    private const string BossPhase2Trigger = "ToPhase2";
    private const string BossPhase2AltTrigger = "ToPhase";
    private const string BossPhase3Trigger = "ToPhase3";

    public static RabbitKillCounter Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        if (tableObject == null)
        {
            tableObject = GameObject.Find("Table");
            if (tableObject != null)
                UnityEngine.Debug.Log("RabbitKillCounter auto-assigned Table object.");
        }

        if (bossAnimator == null)
        {
            GameObject bossObj = GameObject.Find("Boss");
            if (bossObj != null)
                bossAnimator = bossObj.GetComponent<Animator>();

            if (bossAnimator != null)
                UnityEngine.Debug.Log("RabbitKillCounter auto-assigned Boss Animator.");
        }
    }

    bool AnimatorHasParameter(string paramName)
    {
        if (bossAnimator == null)
            return false;

        foreach (AnimatorControllerParameter parameter in bossAnimator.parameters)
        {
            if (parameter.name == paramName)
                return true;
        }

        return false;
    }

    void SetBossTrigger(string triggerName)
    {
        if (bossAnimator == null)
            return;

        if (AnimatorHasParameter(triggerName))
            bossAnimator.SetTrigger(triggerName);
    }

    void SetBossBool(string boolName, bool value)
    {
        if (bossAnimator == null)
            return;

        if (AnimatorHasParameter(boolName))
            bossAnimator.SetBool(boolName, value);
    }

    // Called by bullet when a rabbit is killed
    public void AddKill()
    {
        if (_hasTriggered)
            return;

        _currentKills++;
        UnityEngine.Debug.Log($"Rabbit Kills: {_currentKills}/{requiredKills}");

        if (_currentKills >= requiredKills)
        {
            TriggerTableExplode();
        }
    }

    // Triggers table bloom + destroy + boss phase 2
    void TriggerTableExplode()
    {
        if (_phase2Triggered)
            return;

        _hasTriggered = true;
        _phase2Triggered = true;
        UnityEngine.Debug.Log("✅ Required amount of Rabbits Killed! Table exploding, Boss entering Phase 2");

        // Set phase2Started on BossHealth
        GameObject boss = GameObject.Find("Boss");
        if (boss != null)
        {
            BossHealth bh = boss.GetComponent<BossHealth>();
            if (bh != null)
                bh.phase2Started = true;
        }

        if (tableObject != null)
        {
            Animator tableAnim = tableObject.GetComponent<Animator>();
            if (tableAnim != null)
            {
                tableAnim.SetTrigger("OnExplode");
            }
        }

        SetBossTrigger(BossPhase2Trigger);
        SetBossTrigger(BossPhase2AltTrigger);
        SetBossBool(BossPhase2AltTrigger, true);

        if (tableObject != null)
        {
            Destroy(tableObject, tableDestroyDelay);
        }
    }

    // Get current kill count (for UI)
    public int GetCurrentKills() => _currentKills;
}