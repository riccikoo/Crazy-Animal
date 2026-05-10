using UnityEngine;
using TMPro;
using System.Collections;

public class FloatingText : MonoBehaviour
{
    public float floatSpeed = 100f;
    public float fadeDuration = 1f;

    private TextMeshProUGUI tmp;
    private RectTransform rect;

    void Start()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        rect = GetComponent<RectTransform>();
        StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        float elapsed = 0f;
        Color startColor = tmp.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            rect.anchoredPosition += Vector2.up * floatSpeed * Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            tmp.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }
        Destroy(gameObject);
    }

    public void SetText(string text)
    {
        GetComponent<TextMeshProUGUI>().text = text;
    }
}