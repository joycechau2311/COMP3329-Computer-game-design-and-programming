using UnityEngine;

public class BossFallToGround : MonoBehaviour
{
    [Header("Falling Settings")]
    public float fallSpeed = 8f;
    public float groundY = -3.88f;
    public float gravity = 12f;

    private Vector3 _fallStartPos;
    private bool _isFalling = false;
    private float _currentFallSpeed;

    // Called when boss dies (last hit)
    public void StartFall()
    {
        _fallStartPos = transform.position;
        _isFalling = true;
        _currentFallSpeed = fallSpeed; // Start with initial fall speed instead of 0
        UnityEngine.Debug.Log($"📌 Fall started from position: {_fallStartPos}");
    }

    void Update()
    {
        if (!_isFalling)
            return;

        // Accelerate downward with gravity
        _currentFallSpeed += gravity * Time.deltaTime;
        float newY = transform.position.y - _currentFallSpeed * Time.deltaTime;

        // Stop when reaching ground Y position
        if (newY <= groundY)
        {
            newY = groundY;
            _isFalling = false;
            UnityEngine.Debug.Log($"✅ Boss landed on ground at Y: {newY}");
        }

        // Keep X & Z position from start of fall, only update Y
        transform.position = new Vector3(
            _fallStartPos.x,
            newY,
            _fallStartPos.z
        );
    }

    // Optional: Check if boss has finished falling
    public bool IsFallingComplete()
    {
        return !_isFalling && transform.position.y <= groundY;
    }

    // Optional: Reset fall state (for testing/respawn)
    [ContextMenu("Reset Fall State")]
    public void ResetFall()
    {
        _isFalling = false;
        _currentFallSpeed = 0f;
        transform.position = _fallStartPos;
    }
}