using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 1;
    public int currentHealth;
    public Animator anim;
    public bool isDead = false;
    public bool gotHit = false;

    [Header("Settings")]
    public float hitDuration = 1f; // How long the hit animation stays active

    void Start()
    {
        currentHealth = maxHealth;
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "Level_4" || sceneName.Contains("Boss"))
        {
            maxHealth = 1;
            currentHealth = 1;
        }

        if (anim == null)
            anim = GetComponent<Animator>();

        if (anim == null)
            anim = GetComponentInChildren<Animator>();

        if (anim == null)
            anim = GetComponentInParent<Animator>();

        if (anim == null)
            Debug.LogWarning($"EnemyHealth on '{name}' could not find an Animator component.");
        else
            Debug.Log($"EnemyHealth on '{name}' found Animator '{anim.gameObject.name}'");
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log(name + " hit! Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            StartCoroutine(DieSequence());
        }
        else
        {
            StartCoroutine(PlayHitAnimation());
        }
    }

    IEnumerator PlayHitAnimation()
    {
        gotHit = true;

        if (anim != null)
        {
            Debug.Log($"EnemyHealth '{name}' PlayHitAnimation: has IsGettingHit={HasParameter("IsGettingHit")}, IsGetting={HasParameter("IsGetting")}, GetHit={HasParameter("GetHit")}, Hit={HasParameter("Hit")}, state GetHit={HasState("GetHit")}, Hit={HasState("Hit")}");

            if (HasParameter("IsGettingHit"))
                anim.SetBool("IsGettingHit", true);
            else if (HasParameter("IsGetting"))
                anim.SetBool("IsGetting", true);

            if (HasParameter("GetHit"))
            {
                anim.SetTrigger("GetHit");
            }
            else if (HasParameter("Hit"))
            {
                anim.SetTrigger("Hit");
            }
            else if (HasState("GetHit"))
            {
                anim.CrossFadeInFixedTime("GetHit", 0f);
            }
            else if (HasState("Hit"))
            {
                anim.CrossFadeInFixedTime("Hit", 0f);
            }
            else if (HasState("Hurt"))
            {
                anim.CrossFadeInFixedTime("Hurt", 0f);
            }
            else if (HasState("HurtAnim"))
            {
                anim.CrossFadeInFixedTime("HurtAnim", 0f);
            }
            else if (HasState("Damage"))
            {
                anim.CrossFadeInFixedTime("Damage", 0f);
            }
            else if (HasState("RunAnim"))
            {
                anim.CrossFadeInFixedTime("RunAnim", 0f);
            }
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(hitDuration);

        if (anim != null)
        {
            if (HasParameter("IsGettingHit"))
                anim.SetBool("IsGettingHit", false);
            if (HasParameter("IsGetting"))
                anim.SetBool("IsGetting", false);
        }

        gotHit = false;
    }

    IEnumerator DieSequence()
    {
        Debug.Log(name + " destroyed!");
        isDead = true;

        if (anim != null)
        {
            Debug.Log($"EnemyHealth '{name}' DieSequence: has IsDead={HasParameter("IsDead")}, Dead={HasParameter("Dead")}, Die={HasParameter("Die")}, DeadAnim={HasParameter("DeadAnim")}, state DeadAnim={HasState("DeadAnim")}, Die={HasState("Die")}, GetHit={HasState("GetHit")}");

            if (HasParameter("IsDead"))
            {
                anim.SetBool("IsDead", true);
            }
            else if (HasParameter("Dead"))
            {
                anim.SetBool("Dead", true);
            }
            else if (HasParameter("Die"))
            {
                anim.SetTrigger("Die");
            }
            else if (HasParameter("DeadAnim"))
            {
                anim.SetTrigger("DeadAnim");
            }
            else if (HasState("DeadAnim"))
            {
                anim.Play("DeadAnim");
            }
            else if (HasState("Die"))
            {
                anim.Play("Die");
            }
            else if (HasState("Death"))
            {
                anim.Play("Death");
            }
            else if (HasState("Dead"))
            {
                anim.Play("Dead");
            }
            else if (HasState("GetHit"))
            {
                anim.Play("GetHit");
            }
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        yield return new WaitForSeconds(1.0f);
        BossPhaseController.instance?.AddKilledRabbit();
        Destroy(gameObject);
    }

    private bool HasParameter(string paramName)
    {
        if (anim == null)
            return false;

        foreach (AnimatorControllerParameter parameter in anim.parameters)
        {
            if (parameter.name == paramName)
                return true;
        }

        return false;
    }

    private bool HasState(string stateName)
    {
        if (anim == null)
            return false;

        int stateHash = Animator.StringToHash(stateName);
        return anim.HasState(0, stateHash);
    }
}