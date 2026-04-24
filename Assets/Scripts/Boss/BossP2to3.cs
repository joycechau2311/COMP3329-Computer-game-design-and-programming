using UnityEngine;

public class BossPhase2to3 : MonoBehaviour
{
    public static BossPhase2to3 instance;

    private Animator _anim;
    private BossHealth _bossHealth;
    public float currentGPA; // fallback only

    void Awake()
    {
        instance = this;
        _anim = GetComponent<Animator>();
        _bossHealth = GetComponent<BossHealth>();

        if (_anim == null)
            _anim = GetComponentInChildren<Animator>();

        if (_anim == null)
            _anim = GetComponentInParent<Animator>();

        if (_bossHealth == null)
        {
            GameObject bossObj = GameObject.Find("Boss");
            if (bossObj != null)
                _bossHealth = bossObj.GetComponent<BossHealth>();

            if (_bossHealth == null)
                _bossHealth = FindObjectOfType<BossHealth>();
        }
    }

    void Update()
    {
        if (_anim == null)
            return;

        if (_bossHealth != null)
        {
            float roundedGPA = Mathf.Round(_bossHealth.CurrentHealth * 100f) / 100f;
            _anim.SetFloat("BossGPA", roundedGPA);
            return;
        }

        if (currentGPA != 0f)
        {
            _anim.SetFloat("BossGPA", Mathf.Round(currentGPA * 100f) / 100f);
        }
    }

    // Update GPA from BossHealth
    public void SetGPA(float value)
    {
        currentGPA = value;
    }
}