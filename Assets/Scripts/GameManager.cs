using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int SavedStudents { get; private set; } = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("Duplicate GameManager detected. Destroying this one.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("GameManager successfully initialized (DontDestroyOnLoad)");
    }

    public void AddSavedStudent(int amount = 1)
    {
        SavedStudents += amount;
        Debug.Log($"Saved students count: {SavedStudents}");
    }

    public void LoadNextLevel()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log($"Loading next scene: Index {nextIndex}");
            SceneManager.LoadScene(nextIndex);
        }
        else
        {
            Debug.Log("This is the last level! Going to ending...");
            // 之後可以改成載入 Ending Scene
        }
    }

    // 遊戲結束時可呼叫（boss 組打完後用）
    public void GameOver(bool playerWin)
    {
        // 你可以之後再加 Ending Scene，這裡先 Debug
        Debug.Log(playerWin ? "Good Ending！考試成功" : "Bad Ending…考試泡湯");
        // SceneManager.LoadScene("EndingScene"); // 之後再加
    }
}