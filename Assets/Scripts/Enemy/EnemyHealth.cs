using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 1;
    private int currentHealth;
    public Animator anim;
    public bool isDead = false;
    public bool gotHit = false;

    [Header("Settings")]
    public float hitDuration = 0.1f; // How long the hit animation stays active

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(name + " hit! Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            StartCoroutine(DieSequence());
        }
        else
        {
            // Use a Coroutine to handle the "Hit" timer
            StartCoroutine(PlayHitAnimation());
        }
    }

    IEnumerator PlayHitAnimation()
    {
        anim.SetBool("IsGettingHit", true);
        gotHit = true;

        // Stop movement while hit animation plays
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        yield return new WaitForSeconds(hitDuration);
        anim.SetBool("IsGettingHit", false);
        gotHit = false;
    }

    IEnumerator DieSequence()
    {
        Debug.Log(name + " destroyed!");
        isDead = true;
        anim.SetBool("IsDead", true);

        // Stop movement immediately
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}