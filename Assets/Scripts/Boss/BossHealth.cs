using System.Diagnostics;
using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [Header("Boss Health Settings")]
    public float maxHealth = 4.3f;
    public float damagePerBullet = 0.1f;
    public float invincibilityDuration = 0.2f;
    public float sleepAnimationDelay = 0.5f;
    public float hitAnimationLayer = 1; 
    public float sleepAnimationLength = 2.0f; 
    public bool IsFightActive { get; private set; } = false;

    public event System.Action<float, float> OnHealthChanged;
    public event System.Action OnBossDied;

    [Header("Animation Link")]
    public Animator bossAnimator;

    public float CurrentHealth => _currentHealth;
    public float _currentHealth;
    private bool _isInvincible;
    public bool _isDead;
    public bool phase2Started = false;
    private bool _phase2HasEntered = false;
    public static bool bossIsDead = false;

    private const string BOSS_GPA_PARAM = "BossGPA";
    private BossFallToGround _fallToGround;

    void Start()
    {
        // Initialize core variables
        _currentHealth = maxHealth;
        _isInvincible = false;
        _isDead = false;
        bossIsDead = false;

        // Auto-find Animator component if not assigned
        if (bossAnimator == null)
            bossAnimator = GetComponent<Animator>();
        if (bossAnimator == null)
            bossAnimator = GetComponentInChildren<Animator>();
        if (bossAnimator == null)
            bossAnimator = GetComponentInParent<Animator>();


        // Enable Hit Animation layer (critical for hit animation to work)
        if (bossAnimator != null && bossAnimator.layerCount > hitAnimationLayer)
        {
            bossAnimator.SetLayerWeight((int)hitAnimationLayer, 1f);
        }

        // Initialize fall-to-ground component (auto-add if missing)
        _fallToGround = GetComponent<BossFallToGround>();
        if (_fallToGround == null)
        {
            _fallToGround = gameObject.AddComponent<BossFallToGround>();
        }

        UpdateBossGPAAnimation();}

    void Update()
    {
        UpdateBossGPAAnimation();
    }


    public void TakeDamage()
    {
        if (_isDead || _isInvincible) return;

        _currentHealth -= damagePerBullet;
        _currentHealth = Mathf.Clamp(_currentHealth, 0f, maxHealth);

        if (IsFightActive)
            OnHealthChanged?.Invoke(_currentHealth, maxHealth);

        PlayHitAnimationOnCorrectLayer();
        StartCoroutine(InvincibilityFrames());

        if (_currentHealth <= 0f)
            StartCoroutine(DieAfterHitAnimation());
    }

    // Play hit animation on the correct Animator layer
    private void PlayHitAnimationOnCorrectLayer()
    {
        // Try custom hit animation script first
        BossHitAnimation hitAnim = GetComponent<BossHitAnimation>();
        if (hitAnim != null)
        {
            hitAnim.TriggerHitAnimation();
            return;
        }

        // Fallback: Direct trigger for OnHit parameter
        if (bossAnimator != null && AnimatorHasTrigger("OnHit"))
        {
            bossAnimator.ResetTrigger("OnHit"); // Prevent double triggers
            bossAnimator.SetTrigger("OnHit");
        }
    }

    // Invincibility frames to prevent rapid consecutive damage
    private System.Collections.IEnumerator InvincibilityFrames()
    {
        _isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        _isInvincible = false;
    }

    // Death sequence: Hit anim → Sleep anim → Fall to ground → Hide boss
    private System.Collections.IEnumerator DieAfterHitAnimation()
    {

        if (_isDead) yield break;

        _isDead = true;
        bossIsDead = true;

        // Kill all remaining enemies
        EnemyHealth[] enemies = FindObjectsOfType<EnemyHealth>();
        foreach (EnemyHealth enemy in enemies)
        {
            if (enemy != null && !enemy.isDead)
                enemy.TakeDamage(999); // Instant kill
        }

        // Freeze boss movement
        BossFloatAndFreeze floatScript = GetComponent<BossFloatAndFreeze>();
        if (floatScript != null)
        {
            floatScript.FreezeFloatFor3Seconds();
            floatScript.enabled = false;
        }

        // Wait for hit animation to finish
        yield return new WaitForSeconds(sleepAnimationDelay);

        // Play sleep/death animation on Base Layer (layer 0)
        if (bossAnimator != null)
        {
            int sleepHash = Animator.StringToHash("B_Sleep");
            if (bossAnimator.HasState(0, sleepHash))
            {
                bossAnimator.Play(sleepHash, 0); // Force play on Base Layer
            }
        }

        // Start falling to ground
        if (_fallToGround != null)
        {
            _fallToGround.StartFall();
        }

        // Disable collider to prevent further damage
        Collider2D bossCollider = GetComponent<Collider2D>();
        if (bossCollider != null)
            bossCollider.enabled = false;


        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadEnding();
        }
        else
        {
        }

        // (optional) still notify others if needed
        OnBossDied?.Invoke();

        // Continue animation
        yield return new WaitForSeconds(sleepAnimationLength);

        gameObject.SetActive(false);

    }

    private void UpdateBossGPAAnimation()
    {
        if (bossAnimator == null)
        {
            bossAnimator = GetComponentInChildren<Animator>();
            if (bossAnimator == null) return;
        }

        float roundedHealth = Mathf.Round(_currentHealth * 100f) / 100f;

        if (Mathf.Abs(bossAnimator.GetFloat(BOSS_GPA_PARAM) - roundedHealth) > 0.01f)
        {
            bossAnimator.SetFloat(BOSS_GPA_PARAM, roundedHealth);
        }

        // Sync phase 2 state
        if (BossPhaseController.instance != null)
        {
            phase2Started = BossPhaseController.instance.isPhase2Active;
        }

    }

    private bool _phase3Triggered = false;
    private const string BossPhase3Trigger = "ToPhase3";

    // Trigger phase 3 animation/state
    private void SetBossPhase3Trigger()
    {
        if (_phase3Triggered || bossAnimator == null) return;

        foreach (AnimatorControllerParameter param in bossAnimator.parameters)
        {
            if (param.name == BossPhase3Trigger)
            {
                bossAnimator.SetTrigger(BossPhase3Trigger);
                _phase3Triggered = true;

                // Force play phase 3 spawn animation on Base Layer
                if (bossAnimator.HasState(0, Animator.StringToHash("B_Spawn_P3")))
                {
                    bossAnimator.Play("B_Spawn_P3", 0);
                }
                break;
            }
        }
    }

    // Check if Animator has a specific trigger parameter
    private bool AnimatorHasTrigger(string triggerName)
    {
        if (bossAnimator == null) return false;

        foreach (AnimatorControllerParameter param in bossAnimator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Trigger && param.name == triggerName)
                return true;
        }
        return false;
    }

    // Context menu: Reset boss health to max (for testing)
    [ContextMenu("Reset Health to Max")]
    public void ResetHealth()
    {
        _currentHealth = maxHealth;
        _isDead = false;
        _phase3Triggered = false;
        bossIsDead = false;

        // Trigger health update for UI
        if (IsFightActive)
            OnHealthChanged?.Invoke(_currentHealth, maxHealth);

        UpdateBossGPAAnimation();
    }

    // Context menu: Test damage (for debugging)
    [ContextMenu("Take 0.1 Damage")]
    public void DebugTakeDamage()
    {
        TakeDamage();
    }


    public void ActivateBossFight()
    {
        IsFightActive = true;

        // Send initial UI sync (max -> current)
        OnHealthChanged?.Invoke(_currentHealth, maxHealth);

    }


}