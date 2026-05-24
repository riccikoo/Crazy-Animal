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

    [Header("UI")]
    public GameObject mainMenuUI;
    public GameObject selectCharacterUI;

    [Header("Character")]
    public Transform characterHolder;

    public Transform charPointMainMenu;
    public Transform charPointSelect;
    
    public Transform modelPivot;
    public Vector3 mainModelRotation;
    public Vector3 selectModelRotation;

    private Quaternion targetModelRotation;

    public float moveSpeed = 5f;

    private Transform currentCharPoint;

    void Start()
    {
        targetPoint = mainMenuPoint;
        currentCharPoint = charPointMainMenu;

        selectCharacterUI.SetActive(false);

        targetModelRotation = Quaternion.Euler(mainModelRotation);
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
        mainMenuUI.SetActive(false);

        targetPoint = selectCharacterPoint;

        currentCharPoint = charPointSelect;

        StartCoroutine(ShowCharacterUI());

        targetModelRotation = Quaternion.Euler(selectModelRotation);
    }

    IEnumerator ShowCharacterUI()
    {
        yield return new WaitForSeconds(0.5f);

        selectCharacterUI.SetActive(true);
    }

    public void BackToMainMenu()
    {
        selectCharacterUI.SetActive(false);

        targetPoint = mainMenuPoint;

        currentCharPoint = charPointMainMenu;

        StartCoroutine(ShowMainMenuUI());

        targetModelRotation = Quaternion.Euler(mainModelRotation);
    }

    IEnumerator ShowMainMenuUI()
    {
        yield return new WaitForSeconds(0.5f);

        mainMenuUI.SetActive(true);
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