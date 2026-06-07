using UnityEngine;
using System.Collections;

public class SettingsPopup : MonoBehaviour
{
    public GameObject settingsCanvas;
    public RectTransform settingsPanel;

    public GameObject mainButtons;
    public GameObject selectCharacterButton;

    public Vector2 hiddenPos = new Vector2(0, -900);
    public Vector2 showPos = new Vector2(0, 0);

    public float speed = 8f;

    void Start()
    {
        settingsPanel.anchoredPosition = hiddenPos;
        settingsCanvas.SetActive(false);
    }

        public void OpenSettings()
    {
        Debug.Log("OPEN SETTINGS DIPENCET");

        settingsCanvas.SetActive(true);

        mainButtons.SetActive(false);
        selectCharacterButton.SetActive(false);

        StopAllCoroutines();
        StartCoroutine(MovePanel(showPos));
    }
    
    public void CloseSettings()
    {
        Debug.Log("BACK DIPENCET");
        StopAllCoroutines();
        StartCoroutine(CloseRoutine());
    }

    IEnumerator CloseRoutine()
    {
        mainButtons.SetActive(true);
        selectCharacterButton.SetActive(true);

        yield return MovePanel(hiddenPos);

        settingsCanvas.SetActive(false);
    }

    IEnumerator MovePanel(Vector2 target)
    {
        while (Vector2.Distance(settingsPanel.anchoredPosition, target) > 1f)
        {
            settingsPanel.anchoredPosition = Vector2.Lerp(
                settingsPanel.anchoredPosition,
                target,
                Time.deltaTime * speed
            );

            yield return null;
        }

        settingsPanel.anchoredPosition = target;
    }
}