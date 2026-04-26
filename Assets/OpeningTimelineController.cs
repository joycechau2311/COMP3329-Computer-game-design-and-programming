using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class OpeningTimelineController : MonoBehaviour
{
    public PlayableDirector director;
    public string nextSceneName = "Level_1";

    private void Start()
    {
        if (director == null)
            director = GetComponent<PlayableDirector>();

        director.stopped += OnTimelineFinished;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape))
        {
            SkipCutscene();
        }
    }

    private void OnDestroy()
    {
        if (director != null)
            director.stopped -= OnTimelineFinished;
    }

    private void OnTimelineFinished(PlayableDirector obj)
    {
        LoadNextScene();
    }

    public void SkipCutscene()
    {
        LoadNextScene();
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}