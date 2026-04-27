using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public AudioClip hoverSound;
    public AudioClip clickSound;

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Access the Singleton instance directly
        if (AudioManager.Instance != null && hoverSound != null)
        {
            AudioManager.Instance.PlayUISound(hoverSound);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (AudioManager.Instance != null && clickSound != null)
        {
            AudioManager.Instance.PlayUISound(clickSound);
        }
    }
}