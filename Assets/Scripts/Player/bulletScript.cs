using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletScript : MonoBehaviour
{
    [Header("Tuning")]
    public float speed = 10f;
    [HideInInspector] public float shootDirection = 1f;
    [Header("Damage")]
    public int damage = 1;  // Tune per bullet type later

    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // ADJUSTED ROTATION: 
        // If the sprite faces right by default, we only rotate it 180 degrees if shooting left.
        float rotationZ = shootDirection > 0 ? 0f : 180f;
        transform.rotation = Quaternion.Euler(0, 0, rotationZ);

        // POSITION OFFSET:
        // Keep or remove this based on where your bullet spawns relative to the player's arm.
        Vector3 pos = transform.position;
        pos.y -= 0.5f;
        if (shootDirection > 0)
        {
            pos.x += 0.5f;
        }
        else
        {
            pos.x -= 0.5f;
        }

        transform.position = pos;

        rb.velocity = new Vector2(speed * shootDirection, 0f);
        rb.gravityScale = 0f;

        if (col != null) Invoke(nameof(EnableCollider), 0.01f);
    }

    void EnableCollider()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = true;
    }

    // TRIGGER COLLISION – Destroys ENEMY on hit!
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Bullet hit: " + other.name + " | Tag: " + other.tag);  // TEMP DEBUG

        if (other.CompareTag("Enemy"))
        {
            // Deal damage or just destroy
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
            else
            {
                Destroy(other.gameObject);  // Instant kill if no health script
            }

            // Destroy bullet too (poof effect later)
            Destroy(gameObject);
        }

        if (other.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}