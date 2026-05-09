using UnityEngine;
using UnityEngine.SceneManagement;

public class LandingPageManager : MonoBehaviour
{
    public void GoToDashboard()
    {
        // Ini akan memanggil scene MainMenu kamu
        SceneManager.LoadScene("MainMenu");
    }
}