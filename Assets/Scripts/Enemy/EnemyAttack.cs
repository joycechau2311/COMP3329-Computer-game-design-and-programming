using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public int attackDamage = 1;
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
        else
        {
            Debug.LogWarning($"EnemyAttack on {gameObject.name} cannot find Player with tag 'Player'!");
        }
    }

    public void TryAttack()
    {
        if (!CanAttack()) return;
        if (playerTransform == null)
        {
            Debug.LogWarning("Cannot attack: Player Transform is null!");
            return;
        }

        lastAttackTime = Time.time;

        PlayerHealth ph = playerTransform.GetComponent<PlayerHealth>();
        if (ph != null)
        {
            ph.TakeDamage(attackDamage);
            Debug.Log($"{gameObject.name} attacked player! Damage: {attackDamage}");
        }
        else
        {
            Debug.LogWarning("PlayerHealth component not found on Player!");
        }
    }

    private bool CanAttack()
    {
        return Time.time - lastAttackTime >= attackCooldown;
    }

    // 可選：如果之後想讓敵人追擊時才攻擊，可以加這個公開方法給 EnemyMovement 使用
    public bool IsPlayerInRange(float attackRange)
    {
        if (playerTransform == null) return false;
        return Vector2.Distance(transform.position, playerTransform.position) <= attackRange;
    }
}