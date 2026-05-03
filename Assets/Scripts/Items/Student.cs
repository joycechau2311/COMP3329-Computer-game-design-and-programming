using UnityEngine;

public class Student : MonoBehaviour
{
    private bool isProcessed = false;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isProcessed) return;

        if (collision.CompareTag("Player"))
        {
            SaveStudent();
        }
    }

    private void SaveStudent()
    {
        isProcessed = true;

        if (anim != null) anim.SetTrigger("IsSaved");

        // Update the score here
        if (StudentSaveManager.Instance != null)
        {
            StudentSaveManager.Instance.SaveStudent();
        }

        Destroy(gameObject, 0.5f); // Disappear after touch
    }
}