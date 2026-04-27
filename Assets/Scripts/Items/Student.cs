using UnityEngine;

public class Student : MonoBehaviour
{
    private enum StudentState { Stuck, Saved, Dead }
    private StudentState currentState = StudentState.Stuck;

    private Animator anim;
    private Collider2D studentCollider;

    void Start()
    {
        anim = GetComponent<Animator>();
        studentCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If the student is already saved or dead, ignore further collisions
        if (currentState != StudentState.Stuck) return;

        if (collision.CompareTag("Player"))
        {
            SetSaved();
        }
        else if (collision.CompareTag("Enemy") || collision.CompareTag("Trap"))
        {
            SetDead();
        }
    }

    private void SetSaved()
    {
        currentState = StudentState.Saved;
        anim.SetTrigger("IsSaved");

        // Disable physics so they don't block the player or get hit by enemies
        studentCollider.enabled = false;

        Debug.Log("Student Saved!");

        // Optional: Update your GameManager score
        StudentSaveManager.Instance.SaveStudent();

        // Destroy after the "Saved" animation plays (adjust time as needed)
        Destroy(gameObject, 2f);
    }

    private void SetDead()
    {
        currentState = StudentState.Dead;
        anim.SetTrigger("IsDead");

        studentCollider.enabled = false;

        Debug.Log("Student Died...");

        // Destroy after "Die" animation plays
        Destroy(gameObject, 1.5f);
    }
}