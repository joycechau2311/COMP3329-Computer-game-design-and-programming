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
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
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

    private void OnDestroy()
    {
        // Always unsubscribe to prevent memory leaks or errors
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}