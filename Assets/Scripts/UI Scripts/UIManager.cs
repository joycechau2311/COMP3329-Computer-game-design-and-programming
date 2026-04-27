using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject pauseUI;
    public GameObject gameOverUI;
    public Slider healthSlider;
    public TextMeshProUGUI levelText;
    public Image healthFill;
    public Slider potionSlider;

    [Header("Student Stats")]
    public TextMeshProUGUI studentSavedText; // Drag your TextMeshPro counter here
    
    private bool isPaused = false;

    private void Start()
    {
        UpdateLevel();
        UpdateStudentDisplay(); // Initialize the count on start
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                OnGameResumePressed();
            else
                OnGamePausePressed();
        }

        // Keep the student display updated in real-time
        // Alternatively, call UpdateStudentDisplay() from GameManager when the value changes
        UpdateStudentDisplay();
    }

    public void UpdateStudentDisplay()
    {
        if (studentSavedText != null && GameManager.Instance != null)
        {
            // Pulls the integer from GameManager and displays it
            studentSavedText.text = GameManager.Instance.savedStudents.ToString() + "/" + 10;
        }

    }

    public void OnGamePausePressed()
    {
        pauseUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void OnGameResumePressed()
    {
        pauseUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void OnGameExitPressed()
    {
        Application.Quit();
    }

    public void OnGameResetPressed()
    {
        // If your level resets, you might want to reset the count in GameManager too
        // GameManager.Instance.ResetStudentCount(); 

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        pauseUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        GameManager.Instance.savedStudents = 0;

    }

    public void ShowGameOver()
    {
        gameOverUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void UpdateHealthBar(float current, float max)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = max;
            healthSlider.value = current;

            float healthPercent = (float)current / max;

            if (healthPercent > 0.6f)
                healthFill.color = Color.green;
            else if (healthPercent > 0.3f)
                healthFill.color = Color.yellow;
            else
                healthFill.color = Color.red;
        }
    }

    public void UpdateLevel()
    {
        int currentLevelIndex = SceneManager.GetActiveScene().buildIndex - 1;
        if (levelText != null)
        {
            levelText.text = "Level " + currentLevelIndex;
        }
    }

    public void UpdatePotionBar(float current, float max) // For level 2
    {
        potionSlider.maxValue = max;
        potionSlider.value = current;
    }
}