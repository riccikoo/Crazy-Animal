using UnityEngine;
using System.Collections;

public class SettingsPopup : MonoBehaviour
{
    public GameObject settingsCanvas;
    public RectTransform settingsPanel;

    public CanvasGroup mainMenuGroup;

    public Vector2 hiddenPos = new Vector2(0, -900);
    public Vector2 showPos = new Vector2(0, 0);
    public float speed = 8f;
    public float fadeSpeed = 6f;

    void Start()
    {
        settingsPanel.anchoredPosition = hiddenPos;
        settingsCanvas.SetActive(false);
    }

    public void OpenSettings()
    {
        settingsCanvas.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(OpenRoutine());
    }

    public void CloseSettings()
    {
        StopAllCoroutines();
        StartCoroutine(CloseRoutine());
    }

    IEnumerator OpenRoutine()
    {
        yield return FadeMainMenu(0f, false);
        yield return MovePanel(showPos);
    }

    IEnumerator CloseRoutine()
    {
        yield return MovePanel(hiddenPos);
        settingsCanvas.SetActive(false);
        yield return FadeMainMenu(1f, true);
    }

    IEnumerator FadeMainMenu(float target, bool interactable)
    {
        mainMenuGroup.interactable = false;
        mainMenuGroup.blocksRaycasts = false;

        while (Mathf.Abs(mainMenuGroup.alpha - target) > 0.01f)
        {
            mainMenuGroup.alpha = Mathf.Lerp(mainMenuGroup.alpha, target, Time.deltaTime * fadeSpeed);
            yield return null;
        }

        mainMenuGroup.alpha = target;
        mainMenuGroup.interactable = interactable;
        mainMenuGroup.blocksRaycasts = interactable;
    }

    IEnumerator MovePanel(Vector2 target)
    {
        while (Vector2.Distance(settingsPanel.anchoredPosition, target) > 1f)
        {
            settingsPanel.anchoredPosition = Vector2.Lerp(settingsPanel.anchoredPosition, target, Time.deltaTime * speed);
            yield return null;
        }

        settingsPanel.anchoredPosition = target;
    }
}