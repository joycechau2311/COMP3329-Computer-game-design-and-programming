using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraMovement : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform playerTransform;     // Drag your Player GameObject here in Inspector

    [Header("Offset & Smoothing")]
    public float xOffsetLevel4 = -3f;     // Offset only for Level 4 (player on left)
    public float xOffsetDefault = 0f;     // Offset for other scenes (player centered)
    public float yOffset = 0f;            // Usually 0 or small value to center player vertically
    public float smoothTime = 0.15f;      // How smooth/fast the follow feels (0.1–0.3 is nice)

    private Vector3 velocity = Vector3.zero;  // Used by SmoothDamp

    private void LateUpdate()
    {
        if (playerTransform == null) return;

        // Check if we're in Level 4 and apply the appropriate offset
        string currentScene = SceneManager.GetActiveScene().name;
        float xOffset = (currentScene == "Level_4" || currentScene.Contains("Level4") || currentScene.Contains("Level 4"))
                        ? xOffsetLevel4
                        : xOffsetDefault;

        // Follow player X with scene-specific offset
        Vector3 targetPosition = new Vector3(
            playerTransform.position.x + xOffset,
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