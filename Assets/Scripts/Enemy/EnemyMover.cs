using System.Collections;
using System.Collections.Generic;
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


    private int currentDirection;
    private float halfWidth;
    private Vector2 movement;

    // Start is called before the first frame update
    private void Start()
    {
        health = GetComponent<EnemyHealth>();

        // 新增這兩行
        enemyAttack = GetComponent<EnemyAttack>();
        if (enemyAttack == null)
        {
            Debug.LogWarning($"EnemyAttack component is missing on {gameObject.name}! It will not be able to attack.");
        }

        halfWidth = spriteRenderer.bounds.extents.x;
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
        if ((Physics2D.Raycast(transform.position, Vector2.right, halfWidth + 0.1f, LayerMask.GetMask("Obstacle")) || Physics2D.Raycast(transform.position, Vector2.right, halfWidth + 0.1f, LayerMask.GetMask("Ground"))) && rb.velocity.x > 0)
        {
            currentDirection *= -1;
            spriteRenderer.flipX = true;
        }
        else if ((Physics2D.Raycast(transform.position, Vector2.left, halfWidth + 0.1f, LayerMask.GetMask("Obstacle")) || Physics2D.Raycast(transform.position, Vector2.left, halfWidth + 0.1f, LayerMask.GetMask("Ground"))) && rb.velocity.x < 0)
        {
            currentDirection *= -1;
            spriteRenderer.flipX = false;
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
                Debug.Log($"Enemy {gameObject.name} attacked the player!");
            }
            else
            {
                Debug.LogWarning($"EnemyAttack script is missing on {gameObject.name}");
            }
        }
    }
}
