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
        canvasMainMenu.SetActive(false);

        targetPoint = selectCharacterPoint;
        currentCharPoint = charPointSelect;
        targetModelRotation = Quaternion.Euler(selectModelRotation);

        StartCoroutine(ShowSelectCharacterCanvas());
    }

    IEnumerator ShowSelectCharacterCanvas()
    {
        yield return new WaitForSeconds(0.5f);
        canvasSelectCharacter.SetActive(true);
    }

    public void BackToMainMenu()
    {
        canvasSelectCharacter.SetActive(false);

        targetPoint = mainMenuPoint;
        currentCharPoint = charPointMainMenu;
        targetModelRotation = Quaternion.Euler(mainModelRotation);

        StartCoroutine(ShowMainMenuCanvas());
    }

    IEnumerator ShowMainMenuCanvas()
    {
        yield return new WaitForSeconds(0.5f);
        canvasMainMenu.SetActive(true);
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