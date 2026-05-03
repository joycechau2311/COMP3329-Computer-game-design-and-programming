using System.Diagnostics;
using UnityEngine;

public class EnemyFlyer : MonoBehaviour
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

    [Header("Flying")]
    public Transform groundCheck;
    public LayerMask groundMask;

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
            Vector2 movementDirection = new Vector2(chaseDirection.x, chaseDirection.y).normalized;
            if (movementDirection.sqrMagnitude == 0f)
                movementDirection = Vector2.right;

            rb.velocity = new Vector2(chaseDirection.x * speed, chaseDirection.y * speed);
            if (spriteRenderer != null)
                spriteRenderer.flipX = assetFacesRightByDefault ? (chaseDirection.x < 0) : (chaseDirection.x > 0);
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
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

}