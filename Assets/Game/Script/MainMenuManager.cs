using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    [Header("Camera")]
    public Transform mainMenuPoint;
    public Transform selectCharacterPoint;
    public float cameraSpeed = 3f;

    private Transform targetPoint;

    [Header("Canvas UI")]
    public GameObject canvasMainMenu;
    public GameObject canvasSelectCharacter;

    [Header("Canvas Groups")]
    public CanvasGroup mainMenuGroup;
    public CanvasGroup selectCharacterGroup;
    public float fadeSpeed = 6f;

    [Header("Character")]
    public Transform characterHolder;
    public Transform charPointMainMenu;
    public Transform charPointSelect;
    public float moveSpeed = 5f;

    private Transform currentCharPoint;

    [Header("Model Rotation")]
    public Transform modelPivot;
    public Vector3 mainModelRotation;
    public Vector3 selectModelRotation;

    private Quaternion targetModelRotation;

    void Start()
    {
        targetPoint = mainMenuPoint;
        currentCharPoint = charPointMainMenu;
        targetModelRotation = Quaternion.Euler(mainModelRotation);

        canvasMainMenu.SetActive(true);
        canvasSelectCharacter.SetActive(false);

        mainMenuGroup.alpha = 1;
        selectCharacterGroup.alpha = 0;
    }

    void Update()
    {
        Camera.main.transform.position = Vector3.Lerp(
            Camera.main.transform.position,
            targetPoint.position,
            Time.deltaTime * cameraSpeed
        );

        Camera.main.transform.rotation = Quaternion.Lerp(
            Camera.main.transform.rotation,
            targetPoint.rotation,
            Time.deltaTime * cameraSpeed
        );

        characterHolder.position = Vector3.Lerp(
            characterHolder.position,
            currentCharPoint.position,
            Time.deltaTime * moveSpeed
        );

        modelPivot.localRotation = Quaternion.Lerp(
            modelPivot.localRotation,
            targetModelRotation,
            Time.deltaTime * moveSpeed
        );
    }

    public void OpenSelectCharacter()
    {
        StopAllCoroutines();

        targetPoint = selectCharacterPoint;
        currentCharPoint = charPointSelect;
        targetModelRotation = Quaternion.Euler(selectModelRotation);

        StartCoroutine(SwitchToSelectCharacter());
    }

    IEnumerator SwitchToSelectCharacter()
    {
        yield return FadeCanvas(mainMenuGroup, 0);

        canvasMainMenu.SetActive(false);

        canvasSelectCharacter.SetActive(true);
        selectCharacterGroup.alpha = 0;

        yield return FadeCanvas(selectCharacterGroup, 1);
    }

    public void BackToMainMenu()
    {
        StopAllCoroutines();

        targetPoint = mainMenuPoint;
        currentCharPoint = charPointMainMenu;
        targetModelRotation = Quaternion.Euler(mainModelRotation);

        StartCoroutine(SwitchToMainMenu());
    }

    IEnumerator SwitchToMainMenu()
    {
        yield return FadeCanvas(selectCharacterGroup, 0);

        canvasSelectCharacter.SetActive(false);

        canvasMainMenu.SetActive(true);
        mainMenuGroup.alpha = 0;

        yield return FadeCanvas(mainMenuGroup, 1);
    }

    IEnumerator FadeCanvas(CanvasGroup group, float targetAlpha)
    {
        group.interactable = false;
        group.blocksRaycasts = false;

        while (Mathf.Abs(group.alpha - targetAlpha) > 0.01f)
        {
            group.alpha = Mathf.Lerp(
                group.alpha,
                targetAlpha,
                Time.deltaTime * fadeSpeed
            );

            yield return null;
        }

        group.alpha = targetAlpha;

        bool isVisible = targetAlpha > 0.9f;
        group.interactable = isVisible;
        group.blocksRaycasts = isVisible;
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void Settings()
    {
        SceneManager.LoadScene("Setting");
    }

    public void About()
    {
        SceneManager.LoadScene("About");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}