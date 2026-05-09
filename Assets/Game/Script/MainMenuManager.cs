using UnityEngine;
using UnityEngine.SceneManagement; // Tambahkan ini agar bisa pindah scene

public class MainMenuManager : MonoBehaviour
{
    // Fungsi untuk pindah ke scene game (SampleScene)
    public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    // Fungsi untuk keluar game
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Keluar");
    }
}