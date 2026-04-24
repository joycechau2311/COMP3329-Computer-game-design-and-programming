using UnityEngine;
using UnityEngine.UI;

public class BossHealthBarUI : MonoBehaviour
{
    [Header("Settings")]
    public Slider slider;
    public BossHealth bossHealth;

    void Update()
    {
        if (bossHealth == null || slider == null) return;

        slider.maxValue = bossHealth.maxHealth;
        slider.value = bossHealth._currentHealth;
    }
}