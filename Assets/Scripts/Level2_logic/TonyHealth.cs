using UnityEngine;

public class TonyHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Animator")]
    public Animator tonyAnimator;

    public bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;

        if (tonyAnimator == null)
            tonyAnimator = GetComponent<Animator>();

        // Idle will play automatically because it's the Entry state
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        PlayHitAnimation();

        if (currentHealth <= 0f)
            Die();
    }

    public void PlayHitAnimation()
    {
        if (tonyAnimator != null)
            tonyAnimator.SetTrigger("Hit");
    }

    public void PlayWinAnimation()
    {
        if (tonyAnimator != null)
            tonyAnimator.SetTrigger("Win");
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("Tony has been defeated!");
        // Add death animation here if needed
    }

    public float GetHealthPercentage() => currentHealth / maxHealth;
}