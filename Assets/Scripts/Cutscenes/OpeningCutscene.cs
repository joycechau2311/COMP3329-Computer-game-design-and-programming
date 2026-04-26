using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class OpeningCutscene : MonoBehaviour
{
    [System.Serializable]
    public class CutsceneFrame
    {
        public Sprite image;

        [TextArea(2, 4)]
        public string subtitle;

        public float duration = 3f;
    }

    public Image cutsceneImage;
    public TMP_Text subtitleText;
    public CanvasGroup fadePanel;

    public CutsceneFrame[] frames;

    public string nextSceneName = "Level_1";

    public float fadeDuration = 0.6f;

    private int currentFrame = 0;

    void Start()
    {
        StartCoroutine(PlayCutscene());
    }

    IEnumerator PlayCutscene()
    {
        for (int i = 0; i < frames.Length; i++)
        {
            currentFrame = i;

            cutsceneImage.sprite = frames[i].image;
            subtitleText.text = frames[i].subtitle;

            yield return StartCoroutine(FadeIn());

            yield return new WaitForSeconds(frames[i].duration);

            yield return StartCoroutine(FadeOut());
        }

        SceneManager.LoadScene(nextSceneName);
    }

    IEnumerator FadeIn()
    {
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadePanel.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }

        fadePanel.alpha = 0f;
    }

    IEnumerator FadeOut()
    {
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadePanel.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }

        fadePanel.alpha = 1f;
    }
}