using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class EnemyMover : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform player;
    private EnemyHealth health;
    private EnemyAttack enemyAttack;

    [Header("Settings")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private int startDirection = 1;
    [Tooltip("Mover sprite faces right in the source image.")]
    [SerializeField] private bool spriteFacesRight = true;
    [Tooltip("After rebounding on a box, lock direction for this long to prevent flip-spam.")]
    [SerializeField] private float reboundCommitSeconds = 10f;
    [Tooltip("Minimum time between rebounds to avoid double-flipping (raycast + collision).")]
    [SerializeField] private float minReboundInterval = 0.12f;

    [Header("Obstacle Avoidance")]
    [Tooltip("Layers that the mover will rebound off")]
    [SerializeField] private LayerMask reboundMask;
    [Tooltip("Kept for backward compatibility; if Rebound Mask is not set, this will be used.")]
    [SerializeField] private LayerMask obstacleMask;
    [Tooltip("After rebounding off an obstacle, keep that direction briefly before re-targeting the player.")]
    [SerializeField] private float reboundLockSeconds = 0.8f;

    private int currentDirection;
    private float halfWidth;
    private Vector2 movement;
    private float reboundLockUntil;
    private float reboundCommitUntil;
    private float nextReboundAllowedAt;

    // Start is called before the first frame update
    private void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        health = GetComponent<EnemyHealth>();

        enemyAttack = GetComponent<EnemyAttack>();


        if (spriteRenderer != null)
            halfWidth = spriteRenderer.bounds.extents.x;

        currentDirection = startDirection;
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (health != null && (health.isDead || health.gotHit)) return;

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        // Chase player only when allowed by the rebound commit timer.
        // During the commit window, the mover keeps going "forward" in its rebound direction and will NOT flip
        // unless (1) player is reachable with no obstacles between, or (2) it hits another obstacle and rebounds.
        bool canChasePlayer =
            player != null &&
            (Time.time >= reboundCommitUntil || HasClearPathToPlayer()) &&
            (Time.time >= reboundLockUntil || !IsObstacleInDirection(currentDirection));

        if (canChasePlayer)
        {
            float dx = player.position.x - transform.position.x;
            if (Mathf.Abs(dx) > 0.01f)
            {
                int desiredDirection = dx > 0 ? 1 : -1; // player on right => move right; player on left => move left

                // If there's an obstacle immediately in the desired direction, don't spam flip back and forth.
                // Keep currentDirection until we clear the obstacle (rebound logic handles reversing).
                if (!IsObstacleInDirection(desiredDirection))
                    currentDirection = desiredDirection;
            }
        }

        movement.x = speed * currentDirection;
        movement.y = rb.velocity.y;
        rb.velocity = movement;
        SetFacing();
        ReboundIfObstacleAhead();
    }

    private void SetFacing()
    {
        if (spriteRenderer == null) return;
        spriteRenderer.flipX = spriteFacesRight ? (currentDirection < 0) : (currentDirection > 0);
    }

    private void ReboundIfObstacleAhead()
    {
        LayerMask mask = reboundMask != 0 ? reboundMask : obstacleMask;
        if (mask == 0) return;
        if (Time.time < nextReboundAllowedAt) return;

        Vector2 dir = currentDirection >= 0 ? Vector2.right : Vector2.left;
        bool hit = Physics2D.Raycast(transform.position, dir, halfWidth + 0.1f, mask);
        if (!hit) return;

        currentDirection *= -1;
        reboundLockUntil = Time.time + Mathf.Max(0f, reboundLockSeconds);
        reboundCommitUntil = Time.time + Mathf.Max(0f, reboundCommitSeconds);
        nextReboundAllowedAt = Time.time + Mathf.Max(0f, minReboundInterval);
        SetFacing();
    }

    private bool IsObstacleInDirection(int direction)
    {
        LayerMask mask = reboundMask != 0 ? reboundMask : obstacleMask;
        if (mask == 0) return false;
        Vector2 dir = direction >= 0 ? Vector2.right : Vector2.left;
        return Physics2D.Raycast(transform.position, dir, halfWidth + 0.15f, mask);
    }

    private bool HasClearPathToPlayer()
    {
        if (player == null) return false;
        LayerMask mask = reboundMask != 0 ? reboundMask : obstacleMask;
        if (mask == 0) return true;

        float dx = player.position.x - transform.position.x;
        float dist = Mathf.Abs(dx);
        if (dist <= 0.05f) return true;

        Vector2 dir = dx >= 0 ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, dist, mask);
        return hit.collider == null;
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
        // Never rebound on the player (otherwise player can avoid being attacked).
        if (collision.collider.CompareTag("Player"))
        {
            if (TryGetComponent<EnemyAttack>(out EnemyAttack attack))
                attack.TryAttack();
            return;
        }

        // Rebound off colliders on the rebound/obstacle mask (recommended: boxes only)
        LayerMask mask = reboundMask != 0 ? reboundMask : obstacleMask;
        if (mask != 0)
        {
            int layer = collision.collider.gameObject.layer;
            if (((1 << layer) & mask.value) != 0)
            {
                if (Time.time < nextReboundAllowedAt) return;
                currentDirection *= -1;
                reboundLockUntil = Time.time + Mathf.Max(0f, reboundLockSeconds);
                reboundCommitUntil = Time.time + Mathf.Max(0f, reboundCommitSeconds);
                nextReboundAllowedAt = Time.time + Mathf.Max(0f, minReboundInterval);
                SetFacing();
            }
        }
    }

    public void SetInitialDirection(int direction)
    {
        currentDirection = direction != 0 ? direction : startDirection;
        if (spriteRenderer != null)
            spriteRenderer.flipX = spriteFacesRight ? (currentDirection < 0) : (currentDirection > 0);
    }

    public void SetSpeed(float newSpeed)
    {
        speed = Mathf.Max(0f, newSpeed);
    }
}