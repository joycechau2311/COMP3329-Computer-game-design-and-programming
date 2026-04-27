using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; 

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 5;
    public float currentHealth;
    public bool isDead = false;
    //public bool gotHit = false;
    public float hitDuration = 0.2f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip hitSound;
    public AudioClip deathSound;

    public Animator anim;
    private UIManager uiManager;

    [Header("Game Over Settings")]
    public float delayBeforeGameOver = 1.5f; // The delay parameter you requested
    public string gameOverSceneName = "GameOver"; // Make sure this matches your scene name exactly

    void Start()
    {
        currentHealth = maxHealth;
        uiManager = FindObjectOfType<UIManager>(); 
        
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log("Player got hit! Remaining health: " + currentHealth);
        
        if (uiManager != null)
            uiManager.UpdateHealthBar(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            StartCoroutine(DieSequence());
        }
        else
        {
            // Play regular hit sound
            if (hitSound != null) audioSource.PlayOneShot(hitSound);
            anim.SetTrigger("GetHit");
        }
    }

    IEnumerator DieSequence()
    {
        Debug.Log("Player died!");
        isDead = true;

        // Play death sound
        if (deathSound != null) audioSource.PlayOneShot(deathSound);
        anim.SetTrigger("Dead");

        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;

        yield return new WaitForSeconds(delayBeforeGameOver);

        string currentScene = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString("LastPlayedLevel", currentScene);
        PlayerPrefs.Save(); 

        SceneManager.LoadScene(gameOverSceneName);
    }
}