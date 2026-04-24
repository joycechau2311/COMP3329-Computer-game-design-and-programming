using UnityEngine;

public class BossPhaseController : MonoBehaviour
{
    public static BossPhaseController instance;

    private Animator _anim;
    private int _killedRabbits = 0;
    public int requiredRabbits = 15; // Update to 30 if needed (match your design)
    public bool isPhase2Active = false; // New: Track P2 active state

    void Awake()
    {
        instance = this;
        _anim = GetComponent<Animator>();
    }

    void Start()
    {
        if (_anim == null)
        {
            UnityEngine.Debug.LogError("No Animator component found on Boss!");
            return;
        }

        int stateHash = Animator.StringToHash("B_Spawn_P1");
        if (_anim.HasState(0, stateHash))
        {
            _anim.Play(stateHash);
        }
        else
        {
            UnityEngine.Debug.LogError("State 'B_Spawn_P1' not found in Boss Animator!");
        }
    }

    public void AddKilledRabbit()
    {
        if (isPhase2Active) return; // Lock P2 once activated

        _killedRabbits++;
        UnityEngine.Debug.Log($"Rabbit Kills: {_killedRabbits}/{requiredRabbits}");

        if (_killedRabbits >= requiredRabbits)
        {
            SwitchToShock();
        }
    }

    private bool _hasTriggered = false;
    void SwitchToShock()
    {
        if (_hasTriggered || _anim == null) return;

        _hasTriggered = true;
        isPhase2Active = true; // Mark P2 as permanently active
        _anim.SetTrigger("ToShock");
        _anim.Play("B_Spawn_P2"); // Force play P2 state to prevent revert
        UnityEngine.Debug.Log("BOSS PHASE 2: SHOCK ANIMATION ACTIVATED (PERMANENT)");
    }
}