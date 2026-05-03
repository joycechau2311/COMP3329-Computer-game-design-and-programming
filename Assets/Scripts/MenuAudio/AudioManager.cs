using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

    [Header("Level Music Tracks")]
    [SerializeField] private AudioClip bgmMenu;
    [SerializeField] private AudioClip bgmLevel1;
    [SerializeField] private AudioClip bgmLevel2;
    [SerializeField] private AudioClip bgmLevel3;
    [SerializeField] private AudioClip bgmLevel4;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AudioClip nextClip = null;

        // Check which scene loaded and assign the correct track
        switch (scene.name)
        {
            case "MainMenu":
                nextClip = bgmMenu;
                break;
            case "Level_1":
                nextClip = bgmLevel1;
                break;
            case "Level_2":
                nextClip = bgmLevel2;
                break;
            case "Level_3":
                nextClip = bgmLevel3;
                break;
            case "Level_4":
                nextClip = bgmLevel4;
                break;
        }

        if (nextClip != null)
        {
            // Play the track if we have one and it isn't already playing
            if (musicSource != null && musicSource.clip != nextClip)
            {
                musicSource.clip = nextClip;
                musicSource.Play();
            }
        }
        else
        {
            // Stop the music entirely for unlisted scenes (like Game Over)
            if (musicSource != null && musicSource.isPlaying)
            {
                musicSource.Stop();
                musicSource.clip = null;
            }
        }
    }

    public void PlayUISound(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
}