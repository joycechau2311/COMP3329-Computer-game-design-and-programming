using UnityEngine;

public class LevelEndTrigger : MonoBehaviour
{
    [Header("Settings")]
    //public float delayBeforeTransition = 0.8f;     // Small delay so player feels the gate

    [Header("Optional")]
    public bool playSoundOnEnter = true;

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player touched the gate → Starting transition");

            GameManager.Instance.LoadNextLevel();
        }
    }

    // Optional: Reset trigger (useful if you want to reuse the gate)
    public void ResetTrigger()
    {
        hasTriggered = false;
    }
}