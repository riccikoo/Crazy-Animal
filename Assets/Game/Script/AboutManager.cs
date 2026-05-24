using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AboutManager : MonoBehaviour
{
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}