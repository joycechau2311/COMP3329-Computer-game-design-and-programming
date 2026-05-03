using UnityEngine;
using UnityEngine.SceneManagement;

public class StudentSaveManager : MonoBehaviour
{
    public static StudentSaveManager Instance;

    [Header("Student Save Settings")]
    public int maxStudentsInThisLevel = 10;
    public int savedStudents = 0;

    private string currentSceneName;

    private void Awake()
    {
        Instance = this;
        currentSceneName = SceneManager.GetActiveScene().name;
    }

    public void SaveStudent()
    {
        savedStudents++;

        if (savedStudents > maxStudentsInThisLevel)
            savedStudents = maxStudentsInThisLevel;

        SaveProgress();
    }

    public void SaveProgress()
    {
        if (currentSceneName == "Level_1")
        {
            PlayerPrefs.SetInt("Level1SavedStudents", savedStudents);
        }
        else if (currentSceneName == "Level_3")
        {
            PlayerPrefs.SetInt("Level3SavedStudents", savedStudents);
        }

        PlayerPrefs.Save();
    }

    public static int GetTotalSavedStudents()
    {
        int level1 = PlayerPrefs.GetInt("Level1SavedStudents", 0);
        int level3 = PlayerPrefs.GetInt("Level3SavedStudents", 0);

        return level1 + level3;
    }

    public static void ClearLevelSaveCount(string levelName)
    {
        if (levelName == "Level_1")
        {
            PlayerPrefs.SetInt("Level1SavedStudents", 0);
        }
        else if (levelName == "Level_3")
        {
            PlayerPrefs.SetInt("Level3SavedStudents", 0);
        }

        PlayerPrefs.Save();
    }

    public static void ClearAllSaveCounts()
    {
        PlayerPrefs.SetInt("Level1SavedStudents", 0);
        PlayerPrefs.SetInt("Level3SavedStudents", 0);
        PlayerPrefs.Save();
    }
}