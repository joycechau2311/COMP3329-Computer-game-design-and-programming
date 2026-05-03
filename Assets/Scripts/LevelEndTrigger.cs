using UnityEngine;

public class LevelEndTrigger : MonoBehaviour
{

    [Header("Optional")]
    public bool playSoundOnEnter = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {

            GameManager.Instance.LoadNextLevel();
        }
    }

}
