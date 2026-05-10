using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class XPSystem : MonoBehaviour
{
    [Header("XP Settings")]
    public int currentXP = 0;
    public int xpToNextLevel = 10;
    public int currentLevel = 1;

    [Header("UI Reference")]
    public Slider xpSlider;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI xpText;

    void Start()
    {
        UpdateXPUI();
    }

    public void GainXP(int amount)
    {
        currentXP += amount;
        if (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }
        UpdateXPUI();
    }

    private void LevelUp()
    {
        currentXP -= xpToNextLevel;
        currentLevel++;
        xpToNextLevel = currentLevel * 10;
        Debug.Log("Level Up! Sekarang Level " + currentLevel);
        UpdateXPUI();
    }

    private void UpdateXPUI()
    {
        if (xpSlider) { xpSlider.maxValue = xpToNextLevel; xpSlider.value = currentXP; }
        if (levelText) levelText.text = "Level " + currentLevel;
        if (xpText) xpText.text = currentXP + "/" + xpToNextLevel;
    }
}