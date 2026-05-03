using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackDamage = 1f;
    public float attackCooldown = 1.2f;     

    private float lastAttackTime = 0f;
    private Transform playerTransform;    

    private void Awake()    
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }

    }

    public void TryAttack()
    {
        if (!CanAttack()) return;
        if (playerTransform == null)
        {

            return;
        }

        lastAttackTime = Time.time;

        PlayerHealth ph = playerTransform.GetComponent<PlayerHealth>();
        if (ph != null)
        {
            ph.TakeDamage(attackDamage);

        }

    }

    private bool CanAttack()
    {
        return Time.time - lastAttackTime >= attackCooldown;
    }

    public bool IsPlayerInRange(float attackRange)
    {
        if (playerTransform == null) return false;
        return Vector2.Distance(transform.position, playerTransform.position) <= attackRange;
    }
}