using UnityEngine;
using TMPro;
using System.Collections;

public class LevelUpEffect : MonoBehaviour
{
    public TextMeshProUGUI levelUpText;
    public float displayDuration = 2f;

    void Start()
    {
        levelUpText = GetComponent<TextMeshProUGUI>();
        levelUpText.enabled = false;
    }

    public void ShowLevelUp(int newLevel)
    {
        StopAllCoroutines();
        StartCoroutine(Animate(newLevel));
    }

    IEnumerator Animate(int newLevel)

        levelUpText.text = "LEVEL UP!\nLevel " + newLevel;
        levelUpText.enabled = true;

        float elapsed = 0f;
        Color c = levelUpText.color;

        while (elapsed < displayDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / displayDuration);
            levelUpText.color = new Color(c.r, c.g, c.b, alpha);
            yield return null;
        }

        levelUpText.enabled = false;
    }
}