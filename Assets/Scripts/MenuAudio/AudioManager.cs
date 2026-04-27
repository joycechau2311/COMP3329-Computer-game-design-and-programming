using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

    private void Awake()
    {
        // We keep this so buttons can easily find the manager without expensive searches
        Instance = this;
        
        // We removed the DontDestroyOnLoad logic, so this object 
        // will now be destroyed when you leave the menu scene.
    }

    public void PlayUISound(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
}