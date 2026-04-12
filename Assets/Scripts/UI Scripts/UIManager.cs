using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject pauseUI;
    public GameObject gameOverUI;
    public Slider healthSlider;
    public Image healthFill;

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                OnGameResumePressed();
            else
                OnGamePausePressed();
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
    }

    public void ShowGameOver()
    {
        gameOverUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void UpdateHealthBar(int current, int max)
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
}