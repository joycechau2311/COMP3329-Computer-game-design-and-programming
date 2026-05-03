using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraMovement : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform playerTransform;

    [Header("Offset & Smoothing")]
    public float xOffsetLevel4 = -3f;
    public float xOffsetDefault = 0f;
    public float yOffset = 0f;  
    public float smoothTime = 0.15f;   

    private Vector3 velocity = Vector3.zero;

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
            transform.position.y + yOffset, 
            transform.position.z 
        );

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );
    }
}