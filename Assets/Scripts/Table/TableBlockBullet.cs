using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class TableBlockBullet : MonoBehaviour
{
    [Header("Interception Settings")]
    public float tableMoveSpeed = 15f;
    public float tableYMin = -3.88f;
    public float tableYMax = 2f;
    public Transform bossTransform;
    public string bulletTag = "Bullet";

    [Header("Animation Settings")]
    public Animator tableAnimator;

    private GameObject _closestBullet;
    private float _timeToImpact;
    private float _targetY;
    private bool _isBlocking;
    private int bulletsBlockedCount = 0;
    private bool shouldBlock = true; // Blocks until 30 shots fired

    void Start()
    {
        if (tableAnimator == null)
            tableAnimator = GetComponent<Animator>();

        Collider2D col = GetComponent<Collider2D>();
        Rigidbody2D tableRb = GetComponent<Rigidbody2D>();
        if (col != null)
        {
            col.isTrigger = true;
            Debug.Log("[TABLE] Table collider set to Trigger so enemies can pass through.");
        }
        Debug.Log($"[TABLE] Start() - Collider found: {col != null}, Is Trigger: {(col != null ? col.isTrigger : false)}");
        Debug.Log($"[TABLE] Rigidbody found: {tableRb != null}, Body Type: {(tableRb != null ? tableRb.bodyType.ToString() : "N/A")}");
        Debug.Log($"[TABLE] Animator found: {tableAnimator != null}");
    }

    void Update()
    {
        // Check if we should still be blocking (before 30 rabbit kills)
        if (shouldBlock && RabbitKillCounter.Instance != null)
        {
            if (RabbitKillCounter.Instance.GetCurrentKills() >= 30)
            {
                shouldBlock = false;
                PlayDisappearAnimation();
            }
        }

        if (!shouldBlock)
            return;

        FindClosestThreateningBullet();

        if (_closestBullet != null && !_isBlocking)
        {
            CalculateInterceptPosition();
            MoveTableToIntercept();
        }
        else
        {
            ReturnToIdlePosition();
        }
    }

    void PlayDisappearAnimation()
    {
        Debug.Log("✨ 30 shots reached! Table disappearing...");
        if (tableAnimator != null)
        {
            tableAnimator.SetTrigger("Disappear"); // Make sure this animation exists
        }

        // Disable blocking after animation plays
        StartCoroutine(DisableTableAfterAnimation());
    }

    IEnumerator DisableTableAfterAnimation()
    {
        yield return new WaitForSeconds(1f); // Adjust based on your animation length
        gameObject.SetActive(false); // Hide the table
    }

    void FindClosestThreateningBullet()
    {
        _closestBullet = null;
        float minDist = Mathf.Infinity;
        bool bossIsRight = bossTransform != null && bossTransform.position.x > transform.position.x;

        bulletScript[] bullets = FindObjectsOfType<bulletScript>();
        foreach (bulletScript bullet in bullets)
        {
            if (bullet == null) continue;

            // Check if bullet is approaching table (moving towards boss)
            if (bossTransform != null)
            {
                if (bossIsRight && bullet.shootDirection <= 0) continue;  // Bullet moving left, but boss is right
                if (!bossIsRight && bullet.shootDirection >= 0) continue; // Bullet moving right, but boss is left
            }

            float dist = Vector2.Distance(bullet.transform.position, transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                _closestBullet = bullet.gameObject;
            }
        }
    }

    void CalculateInterceptPosition()
    {
        if (_closestBullet == null || bossTransform == null) return;

        bulletScript comp = _closestBullet.GetComponent<bulletScript>();
        _targetY = _closestBullet.transform.position.y;
        _targetY = Mathf.Clamp(_targetY, tableYMin, tableYMax);

        float dist = Mathf.Abs(bossTransform.position.x - _closestBullet.transform.position.x);
        _timeToImpact = dist / comp.speed;
    }

    void MoveTableToIntercept()
    {
        if (_timeToImpact <= 0) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            new Vector3(transform.position.x, _targetY, transform.position.z),
            tableMoveSpeed * Time.deltaTime
        );
    }

    void ReturnToIdlePosition()
    {
        float idleY = (tableYMin + tableYMax) / 2f;
        transform.position = Vector3.MoveTowards(
            transform.position,
            new Vector3(transform.position.x, idleY, transform.position.z),
            5f * Time.deltaTime
        );
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[TABLE] OnTriggerEnter2D called! Object: {other.gameObject.name}, Tag: {other.tag}, IsBlocking: {_isBlocking}");

        if (_isBlocking)
        {
            Debug.Log($"[TABLE] Already blocking, ignoring collision");
            return;
        }

        Debug.Log($"[TABLE] Checking if collision is a bullet...");

        bool isBullet = other.CompareTag(bulletTag) || other.GetComponent<bulletScript>() != null;
        if (!isBullet)
        {
            Debug.Log($"[TABLE] Collision was not a bullet (tag: {bulletTag}, has bulletScript: {other.GetComponent<bulletScript>() != null})");
            return;
        }

        _isBlocking = true;
        Destroy(other.gameObject);
        Debug.Log("✅ [TABLE] Table blocked bullet!");

        if (tableAnimator != null)
        {
            Debug.Log($"[TABLE] SetTrigger('OnBlock') called to transition to Table_Block animation");
            tableAnimator.SetTrigger("OnBlock");
        }
        else
        {
            Debug.LogWarning("[TABLE] Table Animator is null!");
        }

        StartCoroutine(ResetBlocking());
    }

    IEnumerator ResetBlocking()
    {
        yield return new WaitForSeconds(0.25f);
        _isBlocking = false;
    }
}