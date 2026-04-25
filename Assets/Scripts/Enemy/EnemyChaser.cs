using System.Diagnostics;
using UnityEngine;

public class EnemyChaser : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform player;   // Assign in Inspector or auto-find
    private EnemyHealth health;

    private bool initialFlipX;
    private bool assetFacesRightByDefault;

    [Header("Settings")]
    [SerializeField] private float speed = 3f;

    public float visionRange = 20f;
    private bool canSeePlayer = false;

    [Header("Obstacle Jumping")]
    public float boxCheckDistance = 1.0f;  // how far ahead to detect box
    public LayerMask boxLayer;             // layer of your boxes
    public float jumpForce = 4.5f;         // how high the enemy jumps (reduced so jump arc is less extreme)
    private bool isGrounded = true;        // simple ground check
    public Transform groundCheck;
    public LayerMask groundMask;
    public float radius = 0.5f;

    private void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            initialFlipX = spriteRenderer.flipX;
            assetFacesRightByDefault = !initialFlipX;
        }

        health = GetComponent<EnemyHealth>();

        player = FindPlayerTransform();
    }

    private void Update()
    {
        if (groundCheck != null)
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, radius, groundMask);
        else
            isGrounded = true;
    }

    private void FixedUpdate()
    {
        if (player == null)
            player = FindPlayerTransform();

        if (CanSeePlayer())
        {
            canSeePlayer = true;
        }

        if (canSeePlayer) // once seen trigger chase forever
        {
            // 1. STOP if dead or in hit animation
            if (health != null && (health.isDead || health.gotHit))
            {
                if (rb != null)
                    rb.velocity = Vector2.zero;
                return;
            }

            if (player == null || rb == null)
            {
                if (rb != null)
                    rb.velocity = Vector2.zero;
                return;
            }

            Vector2 chaseDirection = (player.position - transform.position).normalized;
            Vector2 movementDirection = new Vector2(chaseDirection.x, 0f).normalized;
            if (movementDirection.sqrMagnitude == 0f)
                movementDirection = Vector2.right;

            // If box detected ahead, try to jump before hitting it
            if (IsBoxInFront(movementDirection) && isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                UnityEngine.Debug.Log("Chaser jumps over box!");
            }

            // 4. Chase movement
            rb.velocity = new Vector2(chaseDirection.x * speed, rb.velocity.y);
            if (spriteRenderer != null)
                spriteRenderer.flipX = assetFacesRightByDefault ? (chaseDirection.x < 0) : (chaseDirection.x > 0);
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            UnityEngine.Debug.Log("Chaser collided with player!");
            if (TryGetComponent<EnemyAttack>(out EnemyAttack attack))
                attack.TryAttack();
        }
    }

    public void SetSpeed(float newSpeed)
    {
        speed = Mathf.Max(0f, newSpeed);
    }

    bool CanSeePlayer()
    {
        if (player == null)
            return false;

        Vector3 dir = player.position - transform.position;
        float distance = dir.magnitude;

        return distance < visionRange;
    }

    private Transform FindPlayerTransform()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            return p.transform;

        PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
        if (playerHealth != null)
            return playerHealth.transform;

        return null;
    }


    bool IsBoxInFront(Vector2 movementDirection)
    {
        Vector2 direction = movementDirection.x < 0 ? Vector2.left : Vector2.right;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, boxCheckDistance, boxLayer);

        if (hit.collider != null)
        {
            return true;
        }

        return false;
    }

}