using UnityEngine;

public class BossHitAnimation : MonoBehaviour
{
    private Animator _bossAnimator;
    private BossPhaseController _phaseController; // New: Link to phase controller

    void Start()
    {
        _bossAnimator = GetComponent<Animator>();
        if (_bossAnimator == null)
            _bossAnimator = GetComponentInChildren<Animator>();
        if (_bossAnimator == null)
            _bossAnimator = GetComponentInParent<Animator>();

        _phaseController = GetComponent<BossPhaseController>();
        if (_phaseController == null)
            _phaseController = GetComponentInParent<BossPhaseController>();

        if (_bossAnimator == null)
        {
            UnityEngine.Debug.LogError("❌ BossHitAnimation could not find an Animator component on boss, children, or parent!");
        }
    }

    public void TriggerHitAnimation()
    {
        if (_bossAnimator == null) return;

        bool hasOnHit = false;
        foreach (var param in _bossAnimator.parameters)
        {
            if (param.name == "OnHit" && param.type == AnimatorControllerParameterType.Trigger)
            {
                hasOnHit = true;
                break;
            }
        }

        if (hasOnHit)
        {
            _bossAnimator.SetTrigger("OnHit");
            UnityEngine.Debug.Log("💥 Boss got hit, playing hit animation (OnHit trigger set)");

            // Critical: If P2 is active, force revert to P2 after hit animation
            if (_phaseController != null && _phaseController.isPhase2Active)
            {
                // Use a small delay to let hit animation play first
                Invoke(nameof(RevertToP2AfterHit), 0.1f);
            }
        }
        else
        {
            UnityEngine.Debug.LogWarning("💥 Boss got hit, but no 'OnHit' trigger found in animator!");
        }
    }

    // Revert to P2 state after hit animation finishes
    private void RevertToP2AfterHit()
    {
        if (_bossAnimator != null && _phaseController != null && _phaseController.isPhase2Active)
        {
            if (_bossAnimator.HasState(0, Animator.StringToHash("B_Spawn_P2")))
            {
                _bossAnimator.Play("B_Spawn_P2");
                UnityEngine.Debug.Log("🔄 Reverted to P2 state after hit animation");
            }
        }
    }
}