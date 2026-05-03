using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class MainMenuManager : MonoBehaviour
{
    [Header("Settings")]
    public string gameSceneName = "GameScene";
    
    [Tooltip("How long to wait before loading the scene, allowing the click sound to play.")]
    public float loadDelay = 0.3f; // 0.3 seconds is usually perfect for a UI click

    // --- FUNCTION FOR THE START BUTTON ---
    public void StartGame()
    {
        // Start the Coroutine instead of loading instantly
        StartCoroutine(LoadSceneWithDelay("Level_1"));
    }

    public void ShowCredits()
    {
        StartCoroutine(LoadSceneWithDelay("CreditsScene"));
    }

    // --- FUNCTION FOR THE EXIT BUTTON ---
    public void ExitGame()
    {
        StartCoroutine(ExitWithDelay());
    }

    // --- COROUTINES (The Delay Logic) ---
    
    private IEnumerator LoadSceneWithDelay(string sceneName)
    {
        // 1. Wait for the delay time
        yield return new WaitForSeconds(loadDelay);
        
        // 2. Load the scene after the wait is over
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator ExitWithDelay()
    {
        // Wait for the sound to play before quitting
        yield return new WaitForSeconds(loadDelay);
        
        Application.Quit();
    }
}