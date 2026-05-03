using UnityEngine;

public class TrapAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackDamage = 1f;
    public float attackCooldown = 1.2f;

    private float lastAttackTime = 0f;

    // We use a Trigger to detect the player automatically
    private void OnTriggerStay2D(Collider2D collision)
    {
        // Check if the object entering the trap is the Player
        if (collision.CompareTag("Player"))
        {
            TryAttack(collision.gameObject);
        }
    }

    public void TryAttack(GameObject player)
    {
        if (!CanAttack()) return;

        lastAttackTime = Time.time;

        // Get the health component directly from the object that touched the trap
        PlayerHealth ph = player.GetComponent<PlayerHealth>();
        if (ph != null)
        {
            ph.TakeDamage(attackDamage);

        }
    }

    private bool CanAttack()
    {
        // Only allow attack if enough time has passed since the last one
        return Time.time - lastAttackTime >= attackCooldown;
    }
}