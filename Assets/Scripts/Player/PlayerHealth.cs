using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 5;
    public float currentHealth;
    //public Animator anim;
    public bool isDead = false;
    public bool gotHit = false;

    public float hitDuration = 0.2f;

    private UIManager uiManager;

    void Start()
    {
        currentHealth = maxHealth;
        uiManager = FindObjectOfType<UIManager>(); // auto-find in scene
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log("Player got hit! Remaining health: " + currentHealth);
        uiManager.UpdateHealthBar(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            StartCoroutine(DieSequence());
        }
        else
        {
            //StartCoroutine(PlayHitAnimation());
        }
    }

    //IEnumerator PlayHitAnimation()
    //{
    //    anim.SetBool("IsGettingHit", true);
    //    gotHit = true;
    //    yield return new WaitForSeconds(hitDuration);
    //    anim.SetBool("IsGettingHit", false);
    //    gotHit = false;
    //}


    IEnumerator DieSequence()
    {
        Debug.Log("Player died!");
        isDead = true;
        //anim.SetBool("IsDead", true);

        // Freeze movement
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;

        // Wait for death animation
        yield return new WaitForSeconds(1f);

        // Here you can trigger respawn or game over
        // For now, just destroy the player object
        Destroy(gameObject);
    }

        //// Optional: healing
        //public void Heal(int amount)
        //{
        //    if (isDead) return;

        //    currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        //    Debug.Log("Player healed! Health: " + currentHealth);
        //}

}
