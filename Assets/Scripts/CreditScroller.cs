using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // We MUST add this to talk to the Image component!

public class CreditScroller : MonoBehaviour
{
    [Header("Scroll Settings")]
    public float scrollSpeed = 50f;
    public RectTransform textRectTransform;
    public float endPositionY = 2000f; 

    [Header("Skip Settings")]
    public float holdTimeToSkip = 1f; 
    private float currentHoldTime = 0f; 

    [Header("UI Elements")]
    public Image skipCircleFill; // This is the new slot for our circular progress bar

    void Update()
    {
        // 1. Move the text upwards every frame
        textRectTransform.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

        // 2. Check if the credits have finished organically
        if (textRectTransform.anchoredPosition.y >= endPositionY)
        {
            ReturnToMenu();
        }

        // 3. Hold Spacebar to skip logic
        if (Input.GetKey(KeyCode.Space))
        {
            currentHoldTime += Time.deltaTime; 

            // Calculate the percentage (0.0 to 1.0) and apply it to the circle graphic
            if (skipCircleFill != null)
            {
                skipCircleFill.fillAmount = currentHoldTime / holdTimeToSkip;
            }

            if (currentHoldTime >= holdTimeToSkip)
            {
                ReturnToMenu();
            }
        }
        else
        {
            // If they let go, reset the timer and instantly empty the visual circle!
            currentHoldTime = 0f; 
            
            if (skipCircleFill != null)
            {
                skipCircleFill.fillAmount = 0f;
            }
        }
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}