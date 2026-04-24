using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;

    // Remove hard world boundary if using BackgroundGroup scrolling
    // public float worldLeftBoundary = 1.7f;   ← Comment this out

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        transform.Translate(Vector2.right * h * moveSpeed * Time.deltaTime);

        // Optional: soft left limit (only if player goes too far left)
        if (transform.position.x < -8f)
        {
            transform.position = new Vector3(-8f, transform.position.y, transform.position.z);
        }
    }
}