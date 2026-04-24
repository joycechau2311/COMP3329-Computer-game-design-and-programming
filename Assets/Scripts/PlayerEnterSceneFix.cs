using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerEnterSceneFix : MonoBehaviour
{
    public float animTime = 0.6f;
    private Animator anim;
    private PlayerController pc;

    void Awake()
    {
        anim = GetComponent<Animator>();
        pc = GetComponentInParent<PlayerController>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(EntryAnimation());
    }

    IEnumerator EntryAnimation()
    {
        // Wait 1 frame to let scene fully load
        yield return null;

        // STOP player control COMPLETELY
        pc.enabled = false;
        pc.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        // PLAY FALL ANIMATION
        anim.Play("Jumping", 0, 0f);
        yield return new WaitForSeconds(animTime);

        // FORCE back to IDLE
        anim.Play("Idle", 0, 0f);

        // Wait a tiny bit to ensure Idle is stable
        yield return new WaitForSeconds(0.1f);

        // GIVE BACK control to player
        pc.enabled = true;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}