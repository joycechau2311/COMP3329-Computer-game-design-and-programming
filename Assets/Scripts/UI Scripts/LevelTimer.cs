using UnityEngine;
using TMPro;

public class LevelTimer : MonoBehaviour
{
    [Header("Time Settings (per level)")]
    public float level1Time = 120f;   // 2 mins
    public float level2Time = 180f;   // 3 mins
    public float level3Time = 240f;   // 4 mins
    public float level4Time = 300f;   // 5 mins

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
        if (!isTimerRunning || currentTime <= 0)
        {
            if (currentTime <= 0) timerText.text = "00:00";
            return;
        }

        currentTime -= Time.deltaTime;
        UpdateTimerDisplay();
    }

    public void SetTimerByCurrentLevel()
    {
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

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
}