using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class bulletScript : MonoBehaviour
{
    [Header("Tuning")]
    public float speed = 10f;
    [HideInInspector] public float shootDirection = 1f;
    [Header("Damage")]
    public int damage = 1;

    [Header("Boss Scene")]
    public string bossSceneName = "Level_4";

    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Collider2D col = GetComponent<Collider2D>();


        // Avoid immediately colliding with the player when the bullet spawns
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null && col != null)
        {
            Collider2D[] playerColliders = playerObj.GetComponentsInChildren<Collider2D>();
            foreach (Collider2D playerCollider in playerColliders)
            {
                if (playerCollider != null)
                    Physics2D.IgnoreCollision(col, playerCollider, true);
            }
        }

        // Configure rigidbody for bullet behavior
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        if (col != null) col.enabled = false;

        float rotationZ = shootDirection > 0 ? 0f : 180f;
        transform.rotation = Quaternion.Euler(0, 0, rotationZ);

        if (rb != null) rb.velocity = new Vector2(speed * shootDirection, 0f);

        if (col != null) Invoke(nameof(EnableCollider), 0.01f);
    }

    void EnableCollider()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        HandleCollision(other);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        HandleCollision(collision.collider);
    }

    void HandleCollision(Collider2D other)
    {
        if (other == null)
            return;

        if (TryCompareTag(other, "Player"))
            return;


        string currentScene = SceneManager.GetActiveScene().name;
        bool isBossScene = currentScene == bossSceneName || currentScene.Contains("Boss");

        if (isBossScene)
        {
            BossCollisionLogic(other);
        }
        else
        {
            NormalCollisionLogic(other);
        }
    }

    void BossCollisionLogic(Collider2D other)
    {
        bool isEnemy = other.TryGetComponent<EnemyHealth>(out EnemyHealth enemyHealth) || TryCompareTag(other, "Enemy");
        bool isBoss = other.TryGetComponent<BossHealth>(out BossHealth bossHealth) || TryCompareTag(other, "Boss");
        bool isTable = TryCompareTag(other, "Table");

        if (isEnemy)
        {
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);

                if (enemyHealth.currentHealth <= 0)
                {
                    RabbitKillCounter.Instance?.AddKill();
                }
            }
            else
            {
                Destroy(other.gameObject);
                RabbitKillCounter.Instance?.AddKill();
            }

            Destroy(gameObject);
        }
        else if (isBoss)
        {
            if (bossHealth != null)
            {
                bossHealth.TakeDamage();
            }

            Destroy(gameObject);
        }
        else if (isTable)
        {
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void NormalCollisionLogic(Collider2D other)
    {
        if (other.TryGetComponent<EnemyHealth>(out EnemyHealth enemyHealth) || TryCompareTag(other, "Enemy"))
        {
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
            else
            {
                Destroy(other.gameObject);
            }

            Destroy(gameObject);
            return;
        }

        if (TryCompareTag(other, "Obstacle"))
        {
            Destroy(gameObject);
        }
    }

    private bool TryCompareTag(Collider2D other, string tag)
    {
        if (other == null) return false;

        try
        {
            return other.CompareTag(tag);
        }
        catch (UnityException)
        {
            return false;
        }
    }

    void OnBecameInvisible()
    {
        // Delay destruction to ensure we don't destroy mid-collision
        Destroy(gameObject, 0.1f);
    }

}