using UnityEngine;

public class BossFloatAndFreeze : MonoBehaviour
{
    [Header("Floating Settings")]
    public float floatHeight = 2f;     // Y from -2 to +2
    public float hitFloatHeight = 1f;
    public float floatSpeed = 4.188f; // 1.5 seconds full cycle

    [Header("Animator State")]
    [Tooltip("Name of the boss hit animation state.")]
    public string hitStateName = "B_Hit";

    [Tooltip("Animator layer index for the boss animation state.")]
    public int animatorLayer = 0;

    private Vector3 _originalStartPos;
    private Animator _animator;
    private bool _wasInHitState = false;
    private bool _isFrozen = false;
    private float _freezeTimer = 0f;
    private float _defaultFloatHeight;

    void Start()
    {
        _originalStartPos = transform.position;
        _animator = GetComponent<Animator>();
        _defaultFloatHeight = floatHeight;

        if (_animator == null)
            _animator = GetComponentInChildren<Animator>();

        if (_animator == null)
            _animator = GetComponentInParent<Animator>();

    }

    void LateUpdate()
    {
        bool inHitState = IsInHitAnimation();
        if (inHitState)
        {
            _wasInHitState = true;
            floatHeight = hitFloatHeight;
        }

        if (_wasInHitState && !inHitState)
        {
            _wasInHitState = false;
            floatHeight = _defaultFloatHeight;
        }

        if (_isFrozen)
        {
            _freezeTimer += Time.deltaTime;
            if (_freezeTimer >= 3f)
            {
                _isFrozen = false;
                _freezeTimer = 0f;
            }
            return;
        }

        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        Vector3 pos = transform.position;
        pos.y = _originalStartPos.y + yOffset;
        transform.position = pos;
    }

    private bool IsInHitAnimation()
    {
        if (_animator == null)
            return false;

        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(animatorLayer);
        int hitNameHash = Animator.StringToHash(hitStateName);
        return stateInfo.IsName(hitStateName) || stateInfo.shortNameHash == hitNameHash;
    }

    public void FreezeFloatFor3Seconds()
    {
        // Keep compatibility with existing code paths.
        _isFrozen = true;
        _freezeTimer = 0f;
    }
}