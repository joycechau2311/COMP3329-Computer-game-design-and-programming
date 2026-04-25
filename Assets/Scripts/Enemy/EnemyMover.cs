using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class EnemyMover : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private EnemyHealth health;
    private EnemyAttack enemyAttack;

    [Header("Settings")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private int startDirection = 1;

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

        enemyAttack = GetComponent<EnemyAttack>();
        if (enemyAttack == null)
        {
            UnityEngine.Debug.LogWarning($"EnemyAttack component is missing on {gameObject.name}! It will not be able to attack.");
        }

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
        if (obstacleMask == 0) return;

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
            if (TryGetComponent<EnemyAttack>(out EnemyAttack attack))
            {
                attack.TryAttack();
                UnityEngine.Debug.Log($"Enemy {gameObject.name} attacked the player!");
            }
            else
            {
                UnityEngine.Debug.LogWarning($"EnemyAttack script is missing on {gameObject.name}");
            }
        }
    }

    public void SetInitialDirection(int direction)
    {
        currentDirection = direction != 0 ? direction : startDirection;
        if (spriteRenderer != null)
            spriteRenderer.flipX = assetFacesRightByDefault ? (currentDirection < 0) : (currentDirection > 0);
    }

    public void SetSpeed(float newSpeed)
    {
        speed = Mathf.Max(0f, newSpeed);
    }
}