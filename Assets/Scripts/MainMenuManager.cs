using UnityEngine;
using UnityEngine.SceneManagement; // Needed to load the game scene

public class MainMenuManager : MonoBehaviour
{
    [Header("Settings")]
    // TYPE THE EXACT NAME OF YOUR GAME SCENE HERE:
    public string gameSceneName = "GameScene";

    // --- FUNCTION FOR THE START BUTTON ---
    public void StartGame()
    {
        // This tells Tuanjie to load your gameplay scene
        SceneManager.LoadScene(gameSceneName);
    }

    // --- FUNCTION FOR THE EXIT BUTTON ---
    public void ExitGame()
    {
        // This line tells the actual built application (.exe, app) to close.
        Application.Quit();

        // This line is ONLY for testing in the editor.
        // Since Application.Quit() doesn't close the Tuanjie Editor window, 
        // we use this to prove that the button works when you click it!
        Debug.Log("The 'Exit' button was clicked. The game would close now if it was a built application!");
    }
}