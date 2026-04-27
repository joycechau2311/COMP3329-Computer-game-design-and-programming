using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverManager : MonoBehaviour
{
    [Header("Scene Routing")]
    public string fallbackSceneName = "Level1"; // Changed this variable name to make sense
    public string mainMenuSceneName = "MainMenu"; 

    [Header("Audio Settings")]
    public AudioSource uiAudioSource;
    public AudioClip hoverSound;
    public AudioClip clickSound;
    public float transitionDelay = 0.5f; 

    public void PlayHoverSound()
    {
        if (uiAudioSource != null && hoverSound != null)
        {
            uiAudioSource.PlayOneShot(hoverSound);
        }
    }

    public void RetryLevel()
    {
        // Ask PlayerPrefs for the last level. If it doesn't exist, use the fallback.
        string sceneToLoad = PlayerPrefs.GetString("LastPlayedLevel", fallbackSceneName);

        StudentSaveManager.ClearLevelSaveCount(sceneToLoad);

        StartCoroutine(PlaySoundAndLoadScene(sceneToLoad));
    }

    public void ReturnToMainMenu()
    {
        StartCoroutine(PlaySoundAndLoadScene(mainMenuSceneName));
    }

    private IEnumerator PlaySoundAndLoadScene(string targetScene)
    {
        if (uiAudioSource != null && clickSound != null)
        {
            uiAudioSource.PlayOneShot(clickSound);
        }

        yield return new WaitForSecondsRealtime(transitionDelay);

        SceneManager.LoadScene(targetScene);
    }
}