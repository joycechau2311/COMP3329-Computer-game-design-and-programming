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

        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddSavedStudent()
    {
        savedStudents++;

    }

    public void ResetStudentCount()
    {
        savedStudents = 0;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        if (transitionAnimator == null)
        {

            GameObject transitionObj = GameObject.FindGameObjectWithTag("Transition");
            if (transitionObj != null)
            {
                transitionAnimator = transitionObj.GetComponent<Animator>();
            }
        }

    }

    public void LoadNextLevel()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;

        string currentSceneName = SceneManager.GetActiveScene().name;

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

        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(levelIndex);
    }

    public void LoadEnding()
    {
        int totalSaved = StudentSaveManager.GetTotalSavedStudents();


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
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


}