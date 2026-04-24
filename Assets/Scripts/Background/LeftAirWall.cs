using UnityEngine;

public class LeftAirWall : MonoBehaviour
{
    [Header("References")]
    public Transform playerTransform; // Drag Player here in Inspector
    public float disableThresholdX = 5f; // Player X position to disable wall

    private Collider2D _airWallCollider;

    private void Start()
    {
        _airWallCollider = GetComponent<Collider2D>();

        // Auto-find player if not assigned
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    private void Update()
    {
        // Check if player passed the threshold
        if (playerTransform != null && playerTransform.position.x > disableThresholdX)
        {
            // Disable collision permanently
            _airWallCollider.enabled = false;
        }
    }
}