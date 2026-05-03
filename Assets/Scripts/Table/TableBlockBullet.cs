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

        }

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

        if (tableAnimator != null)
        {
            tableAnimator.SetTrigger("Disappear"); 
        }

        // Disable blocking after animation plays
        StartCoroutine(DisableTableAfterAnimation());
    }

    IEnumerator DisableTableAfterAnimation()
    {
        yield return new WaitForSeconds(1f);
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
                if (bossIsRight && bullet.shootDirection <= 0) continue;
                if (!bossIsRight && bullet.shootDirection >= 0) continue;
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


        if (_isBlocking)
        {

            return;
        }


        bool isBullet = other.CompareTag(bulletTag) || other.GetComponent<bulletScript>() != null;
        if (!isBullet)
        {
            return;
        }

        _isBlocking = true;
        Destroy(other.gameObject);

        if (tableAnimator != null)
        {
            tableAnimator.SetTrigger("OnBlock");
        }
        else
        {
        }

        StartCoroutine(ResetBlocking());
    }

    IEnumerator ResetBlocking()
    {
        yield return new WaitForSeconds(0.25f);
        _isBlocking = false;
    }
}