using TMPro;
using UnityEngine;

public class SubtitleController : MonoBehaviour
{
    public TextMeshProUGUI subtitleText;

    public void SetSubtitle(string line)
    {
        subtitleText.text = line;
    }

    public void ClearSubtitle()
    {
        subtitleText.text = "";
    }
}