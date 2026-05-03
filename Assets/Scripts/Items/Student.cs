using UnityEngine;

public class Student : MonoBehaviour
{
    private bool isProcessed = false;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Requirement: Student disappears only on physical touch
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
        Debug.Log("Student Touched and Saved!");

        if (anim != null) anim.SetTrigger("IsSaved");

        // Update the score here
        if (StudentSaveManager.Instance != null)
        {
            StudentSaveManager.Instance.SaveStudent();
        }

        Destroy(gameObject, 0.5f); // Disappear after touch
    }
}