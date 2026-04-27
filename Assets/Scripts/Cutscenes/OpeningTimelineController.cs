using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class OpeningTimelineController : MonoBehaviour
{
    public PlayableDirector director;
    public string nextSceneName = "MainMenu";

    [Header("Skip Settings")]
    public float holdTimeToSkip = 1f;
    private float currentHoldTime = 0f;
    private bool isLoadingScene = false;

    [Header("UI Elements")]
    public Image skipCircleFill;

    private void Start()
    {
        if (director == null)
            director = GetComponent<PlayableDirector>();

        if (director != null)
            director.stopped += OnTimelineFinished;

        if (skipCircleFill != null)
            skipCircleFill.fillAmount = 0f;
    }

    private void Update()
    {
        if (isLoadingScene)
            return;

        if (Input.GetKey(KeyCode.Space))
        {
            currentHoldTime += Time.deltaTime;

            if (skipCircleFill != null)
                skipCircleFill.fillAmount = currentHoldTime / holdTimeToSkip;

            if (currentHoldTime >= holdTimeToSkip)
                SkipCutscene();
        }
        else
        {
            currentHoldTime = 0f;

            if (skipCircleFill != null)
                skipCircleFill.fillAmount = 0f;
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
        if (isLoadingScene)
            return;

        isLoadingScene = true;
        SceneManager.LoadScene(nextSceneName);
    }
}