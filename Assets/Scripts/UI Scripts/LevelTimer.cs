using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // Required for changing scenes

public class LevelTimer : MonoBehaviour
{
    [Header("Time Settings (per level)")]
    public float level1Time = 120f;   // 2 mins
    public float level2Time = 180f;   // 3 mins
    public float level3Time = 120f;   // 4 mins
    public float level4Time = 180f;   // 5 mins

    [Header("Game Over Settings")]
    public string gameOverSceneName = "GameOver"; // Make sure this matches your scene name

    private float currentTime;
    private bool isTimerRunning = true;
    private TextMeshProUGUI timerText;

    void Awake()
    {
        timerText = GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        SetTimerByCurrentLevel();
    }

    void Update()
    {
        // If the timer is stopped, do nothing
        if (!isTimerRunning) return;

        // If time runs out
        if (currentTime <= 0)
        {
            currentTime = 0;
            timerText.text = "00:00";
            isTimerRunning = false; // Stop the timer so this only triggers once
            TriggerGameOver();
            return;
        }

        currentTime -= Time.deltaTime;
        UpdateTimerDisplay();
    }

    public void SetTimerByCurrentLevel()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene.Contains("1")) currentTime = level1Time;
        else if (currentScene.Contains("2")) currentTime = level2Time;
        else if (currentScene.Contains("3")) currentTime = level3Time;
        else if (currentScene.Contains("4")) currentTime = level4Time;
        else currentTime = 60f;

        UpdateTimerDisplay();
    }

    void UpdateTimerDisplay()
    {
        int min = Mathf.FloorToInt(currentTime / 60);
        int sec = Mathf.FloorToInt(currentTime % 60);
        timerText.text = $"{min:00}:{sec:00}";
    }

    void TriggerGameOver()
    {
        Debug.Log("Time's up! GPA crashed!");

        // Save the current level name so the "Retry" button knows where to go
        string currentScene = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString("LastPlayedLevel", currentScene);
        PlayerPrefs.Save();

        // Load the Game Over Scene
        SceneManager.LoadScene(gameOverSceneName);
    }
}