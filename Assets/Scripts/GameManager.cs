using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Scene Transition")]
    public Animator transitionAnimator;
    public float transitionTime = 1.2f;

    [Header("Game Status")]
    public int savedStudents = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;

            Debug.Log("✅ GameManager initialized");
        }
        else
        {
            Debug.LogWarning("⚠️ Duplicate GameManager destroyed");
            Destroy(gameObject);
        }
    }

    public void AddSavedStudent()
    {
        savedStudents++;
        Debug.Log($"Student Saved! Total: {savedStudents}");

        // This is where you would trigger a UI update if you have one
        //UIManager.UpdateStudentDisplay(savedStudents);
    }

    public void ResetStudentCount()
    {
        savedStudents = 0;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // CRITICAL: Every time a scene loads, we must find the NEW animator in that scene
        if (transitionAnimator == null)
        {
            // Make sure your Transition Canvas/Object has the Tag "Transition"
            GameObject transitionObj = GameObject.FindGameObjectWithTag("Transition");
            if (transitionObj != null)
            {
                transitionAnimator = transitionObj.GetComponent<Animator>();
            }
        }

        Debug.Log($"Scene: {scene.name} | Animator Assigned: {transitionAnimator != null}");
    }

    public void LoadNextLevel()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;

        string currentSceneName = SceneManager.GetActiveScene().name;

        // If this is the FINAL LEVEL → go to ending
        if (currentSceneName == "Level_4")
        {
            LoadEnding();
            return;
        }

        int nextIndex = currentIndex + 1;

        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            StartCoroutine(LoadLevelWithTransition(nextIndex));
        }
    }

    IEnumerator LoadLevelWithTransition(int levelIndex)
    {
        if (transitionAnimator != null)
        {
            transitionAnimator.SetTrigger("Start");
        }
        else
        {
            Debug.LogWarning("No Animator found in this scene! Loading instantly.");
        }

        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(levelIndex);
    }

    public void LoadEnding()
    {
        int totalSaved = StudentSaveManager.GetTotalSavedStudents();

        Debug.Log("Total Saved Students: " + totalSaved);

        if (totalSaved <= 5)
        {
            SceneManager.LoadScene("Ending_Bad");
        }
        else if (totalSaved >= 16)
        {
            SceneManager.LoadScene("Ending_True");
        }
        else
        {
            SceneManager.LoadScene("Ending_Normal");
        }
    }

    private void OnDestroy()
    {
        // Always unsubscribe to prevent memory leaks or errors
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


}