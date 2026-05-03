using UnityEngine;

public class Bubble : MonoBehaviour
{
    public float visionRange = 5f; // Reduced for better gameplay feel
    private Transform playerTransform;
    private bool isPopped = false;
    private Animator bubbleAnim;

    void Start()
    {
        bubbleAnim = GetComponent<Animator>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;
    }

    void Update()
    {
        if (isPopped || playerTransform == null) return;

        // Requirement: Pop when player is inside "vision" (distance)
        float distance = Vector2.Distance(transform.position, playerTransform.position);
        if (distance <= visionRange)
        {
            PopBubble();
        }
    }

    private void PopBubble()
    {
        isPopped = true;
        if (bubbleAnim != null) bubbleAnim.SetTrigger("PlayerTouch");

        // Release the student so they stay in the world when the bubble is gone
        transform.DetachChildren();

        // Disable the bubble's collider so it doesn't block the player
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Destroy(gameObject, 0.5f);
    }
}