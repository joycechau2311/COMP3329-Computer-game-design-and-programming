using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEndTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        Debug.Log("LevelEndTrigger: Player entered the end trigger!");

        // 安全檢查 GameManager
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance is NULL! Make sure GameManager exists in the first scene and has DontDestroyOnLoad.");
            return;
        }

        // 可選：加一點延遲或淡出效果（之後再加）
        GameManager.Instance.LoadNextLevel();
        Debug.Log("Loading next level...");
    }
}