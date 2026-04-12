using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform playerTransform;     // Drag your Player GameObject here in Inspector

    [Header("Offset & Smoothing")]
    public float yOffset = 0f;            // Usually 0 or small value to center player vertically
    public float smoothTime = 0.15f;      // How smooth/fast the follow feels (0.1–0.3 is nice)

    private Vector3 velocity = Vector3.zero;  // Used by SmoothDamp

    private void LateUpdate()
    {
        if (playerTransform == null) return;

        // Only follow on X-axis, keep fixed Y and Z
        Vector3 targetPosition = new Vector3(
            playerTransform.position.x,     // Follow player's X
            transform.position.y + yOffset, // Keep camera's current Y (or fixed value)
            transform.position.z            // Usually -10 for 2D
        );

        // Smooth movement (prevents jitter)
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );
    }
}