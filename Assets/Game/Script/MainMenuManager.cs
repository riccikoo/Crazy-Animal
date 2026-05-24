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

    void Start()
    {
        targetPoint = mainMenuPoint;

        selectCharacterUI.SetActive(false);
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
    }

    public void OpenSelectCharacter()
    {
        mainMenuUI.SetActive(false);

        targetPoint = selectCharacterPoint;

        StartCoroutine(ShowCharacterUI());
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

        StartCoroutine(ShowMainMenuUI());
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
}