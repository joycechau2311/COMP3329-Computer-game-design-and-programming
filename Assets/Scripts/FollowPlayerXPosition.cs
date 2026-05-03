using UnityEngine;

public class FollowPlayerXPosition : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform playerTransform;
    public float xOffset = 0f; 

    [Header("Horizontal Bounds")]
    public float minX = float.NegativeInfinity;
    public float maxX = float.PositiveInfinity;

    private float _initialY;
    private float _initialZ;

    private void Start()
    {
        // Store the initial Y and Z positions so they never change
        _initialY = transform.position.y;
        _initialZ = transform.position.z;

    }

    private void LateUpdate()
    {
        if (playerTransform == null) return;

        float targetX = playerTransform.position.x + xOffset;
        targetX = Mathf.Clamp(targetX, minX, maxX);

        // Only update X position based on player.
        // Preserve current Y so boss float movement can still work.
        transform.position = new Vector3(
            targetX,
            transform.position.y,
            _initialZ
        );
    }
}
