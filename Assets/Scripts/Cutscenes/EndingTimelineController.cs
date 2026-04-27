using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class EndingTimelineController : MonoBehaviour
{
    public PlayableDirector director;
    public string nextSceneName = "MainMenu";

    private bool hasEnded = false;

    private void Start()
    {
        if (director == null)
            director = GetComponent<PlayableDirector>();

        director.stopped += OnTimelineStopped;
    }

    private void OnTimelineStopped(PlayableDirector obj)
    {
        if (hasEnded) return;

        hasEnded = true;
        SceneManager.LoadScene(nextSceneName);
    }

    private void OnDestroy()
    {
        if (director != null)
            director.stopped -= OnTimelineStopped;
    }
}