using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class EnermyMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float speed = 3f;
    [SerializeField] private int startDirection = 1;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackCooldown = 1f;
    private float lastAttackTime;
    private EnemyHealth health;

    [Header("Obstacle Avoidance")]
    [SerializeField] private LayerMask obstacleMask;

    private bool initialFlipX;
    private bool assetFacesRightByDefault;
    private int currentDirection;
    private float halfWidth;
    private Vector2 movement;

    // Start is called before the first frame update
    private void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        health = GetComponent<EnemyHealth>();
        if (spriteRenderer != null)
        {
            halfWidth = spriteRenderer.bounds.extents.x;
            initialFlipX = spriteRenderer.flipX;
            assetFacesRightByDefault = startDirection >= 0 ? !initialFlipX : initialFlipX;
        }
        currentDirection = startDirection;

    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (health != null && (health.isDead || health.gotHit)) return;
        movement.x = speed * currentDirection;
        movement.y = rb.velocity.y;
        rb.velocity = movement;
        SetDirection();
    }

    private void SetDirection()
    {
        if (obstacleMask == 0 || rb == null) return;

        if (currentDirection > 0 && Physics2D.Raycast(transform.position, Vector2.right, halfWidth + 0.1f, obstacleMask))
        {
            currentDirection = -1;
            if (spriteRenderer != null)
                spriteRenderer.flipX = assetFacesRightByDefault ? true : false;
        }
        else if (currentDirection < 0 && Physics2D.Raycast(transform.position, Vector2.left, halfWidth + 0.1f, obstacleMask))
        {
            currentDirection = 1;
            if (spriteRenderer != null)
                spriteRenderer.flipX = assetFacesRightByDefault ? false : true;
        }
    }

    private void OnTriggerEnter2D(Collider2D obj)
    {
        // Bullet collision stays the same
        if (obj.gameObject.name == "Bullet_001(Clone)")
        {
            Destroy(obj.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.collider.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);
                UnityEngine.Debug.Log("Enemy collided with player! Remaining health: " + playerHealth.currentHealth);
            }
        }
    }

    public void SetSpeed(float newSpeed)
    {
        speed = Mathf.Max(0f, newSpeed);
    }

    public void SetInitialDirection(int direction)
    {
        currentDirection = direction != 0 ? direction : startDirection;
        if (spriteRenderer != null)
            spriteRenderer.flipX = assetFacesRightByDefault ? (currentDirection < 0) : (currentDirection > 0);
    }
}
