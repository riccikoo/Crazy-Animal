using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AboutManager : MonoBehaviour
{
    void Start()
    {
        // 💾 BACA MEMORI GLOBAL: Ambil status terakhir dari Main Menu (0 = Bunyi, 1 = Bisu)
        bool isMusicMuted = (PlayerPrefs.GetInt("MusicMuted", 0) == 1);
        bool isSfxMuted = (PlayerPrefs.GetInt("SFXMuted", 0) == 1);

        // 🎧 Cari semua AudioSource yang bertebaran di scene Credits ini gess
        AudioSource[] allAudioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);

        foreach (AudioSource source in allAudioSources)
        {
            // Kita absen berdasarkan nama objeknya di Hierarchy gess
            if (source.gameObject.name == "BGM")
            {
                source.mute = isMusicMuted; // Kalau objeknya bernama "BGM", ikat ke memori musik
            }
            else
            {
                source.mute = isSfxMuted; // Sisanya (suara tombol dll) wajib ikut status SFX gess
            }
        }
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}