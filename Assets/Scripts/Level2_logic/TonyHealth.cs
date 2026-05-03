using UnityEngine;
using UnityEngine.UI;

public class TonyHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Heart UI")]
    public int maxHearts = 5;
    public Image[] heartImages;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    [Header("Animator")]
    public Animator tonyAnimator;

    public bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;

        if (tonyAnimator == null)
            tonyAnimator = GetComponent<Animator>();

        UpdateHearts();
        // Idle will play automatically because it's the Entry state
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        PlayHitAnimation();
        UpdateHearts();

        if (currentHealth <= 0)
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

    }

    public float GetHealthPercentage() => currentHealth / maxHealth;

    void UpdateHearts()
    {
        float healthPerHeart = maxHealth / maxHearts;
        int heartsRemaining = Mathf.CeilToInt(currentHealth / healthPerHeart);

        for (int i = 0; i < heartImages.Length; i++)
        {
            heartImages[i].sprite = (i < heartsRemaining) ? fullHeart : emptyHeart;
        }
    }
}