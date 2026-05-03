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
    public TextMeshProUGUI studentSavedText; 
    
    private bool isPaused = false;

    [Header("Boss UI")]
    public GameObject bossHealthUI;
    public Slider bossHealthSlider;

    private void Start()
    {
        UpdateLevel();
        UpdateStudentDisplay(); // Initialize the count on start

        ShowBossUI(false);

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


        UpdateStudentDisplay();
    }

    public void UpdateStudentDisplay()
    {


        if (studentSavedText == null) return;

        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "Level_1" || sceneName == "Level_3")
        {
            if (StudentSaveManager.Instance != null)
            {
                studentSavedText.gameObject.SetActive(true);
                studentSavedText.text = StudentSaveManager.Instance.savedStudents + "/10";
            }
            else
            {
                studentSavedText.text = "0/10";
            }
        }
        else
        {

            studentSavedText.gameObject.SetActive(false);
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

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        pauseUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        StudentSaveManager.ClearLevelSaveCount(SceneManager.GetActiveScene().name);

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

    public void ShowBossUI(bool show)
    {
        if (bossHealthUI != null)
            bossHealthUI.SetActive(show);
    }

    public void UpdateBossHealth(float current, float max)
    {
        if (bossHealthSlider == null) return;

        bossHealthSlider.maxValue = max;
        bossHealthSlider.value = current;
    }
}