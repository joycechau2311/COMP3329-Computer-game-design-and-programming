using UnityEngine;

public class LevelEndTrigger : MonoBehaviour
{

    [Header("Optional")]
    public bool playSoundOnEnter = true;

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {

            GameManager.Instance.LoadNextLevel();
        }
    }

    // Optional: Reset trigger (useful if you want to reuse the gate)
    public void ResetTrigger()
    {
        hasTriggered = false;
    }
}