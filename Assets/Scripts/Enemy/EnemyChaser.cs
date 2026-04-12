using UnityEngine;

public class EnemyChaserMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform player;   // Assign in Inspector or auto-find
    private EnemyHealth health;

    [Header("Settings")]
    [SerializeField] private float speed = 3f;

    public float visionRange = 8f;
    private bool canSeePlayer = false;

    [Header("Obstacle Jumping")]
    public float boxCheckDistance = 1.0f;  // how far ahead to detect box
    public LayerMask boxLayer;             // layer of your boxes
    public float jumpForce = 7f;           // how high the enemy jumps
    private bool isGrounded = true;        // simple ground check
    public Transform groundCheck;
    public LayerMask groundMask;
    public float radius = 0.5f;

    private void Start()
    {
        health = GetComponent<EnemyHealth>();
    }

    private void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, radius, groundMask);
    }

    private void FixedUpdate()
    {
        if (CanSeePlayer())
        {
            canSeePlayer = true;
        }

        if (canSeePlayer) // once seen trigger chase forever
        {
            // 1. STOP if dead or in hit animation
            if (health != null && (health.isDead || health.gotHit))
            {
                rb.velocity = Vector2.zero;
                return;
            }

            // 2. Auto-find player
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
                player = p.transform;

            if (player == null)
            {
                rb.velocity = Vector2.zero;
                return;
            }

            // If box detected, try to jump before hitting it
            if (IsBoxInFront() && isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                Debug.Log("Chaser jumps over box!");
            }

            // 4. Chase movement
            Debug.Log("Chasing player");
            Vector2 direction = (player.position - transform.position).normalized;
            rb.velocity = new Vector2(direction.x * speed, rb.velocity.y);
            spriteRenderer.flipX = direction.x < 0;
        }

    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Debug.Log("Chaser collided with player!");
            GetComponent<EnemyAttack>().TryAttack();  // attack on hit
        }

        if (collision.collider.CompareTag("Ground")) // stop if collide with wall
        {
            rb.velocity = Vector2.zero;
        }
    }

    bool CanSeePlayer()
    {
        Vector3 dir = player.position - transform.position;
        float distance = dir.magnitude;

        return distance < visionRange; 
    }


    bool IsBoxInFront()
    {
        Vector2 direction = spriteRenderer.flipX ? Vector2.left : Vector2.right;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, boxCheckDistance, boxLayer);

        if (hit.collider != null)
        {
            //Debug.DrawLine(transform.position, transform.position + (Vector3)direction * boxCheckDistance, Color.red);
            return true;
        }

        //Debug.DrawLine(transform.position, transform.position + (Vector3)direction * boxCheckDistance, Color.green);
        return false;
    }

}
