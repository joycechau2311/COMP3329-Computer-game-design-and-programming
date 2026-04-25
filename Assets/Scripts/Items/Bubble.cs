using UnityEngine;

public class Bubble : MonoBehaviour
{
    [Header("Detection Settings")]
    public float visionRange = 10f; // How close the player needs to be
    private Transform playerTransform;

    private Animator bubbleAnim;
    private bool isPopped = false;

    void Start()
    {
        bubbleAnim = GetComponent<Animator>();

        // Find the player at the start
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    void Update()
    {
        // Don't do anything if already popped or if player is missing
        if (isPopped || playerTransform == null) return;

        // Calculate the distance between bubble and player
        float distance = Vector2.Distance(transform.position, playerTransform.position);

        if (distance <= visionRange)
        {
            PopBubble();
        }
    }

    private void PopBubble()
    {
        isPopped = true;

        if (bubbleAnim != null)
        {
            bubbleAnim.SetTrigger("PlayerTouch");
        }

        // Release the student inside
        transform.DetachChildren();

        // Disable collider so player can walk through the popping effect
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Destroy(gameObject, 1.5f);
        Debug.Log($"Distance check passed: {gameObject.name} popped!");
    }

    // Visual aid in the Editor so you can see the "Vision Range"
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, visionRange);
    }
}