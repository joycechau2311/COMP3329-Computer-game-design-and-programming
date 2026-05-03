using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public Image skipCircleFill;

    void Update()
    {

        textRectTransform.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;


        if (textRectTransform.anchoredPosition.y >= endPositionY)
        {
            ReturnToMenu();
        }

        if (Input.GetKey(KeyCode.Space))
        {
            currentHoldTime += Time.deltaTime; 


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