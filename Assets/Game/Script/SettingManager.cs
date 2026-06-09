using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    public GameObject settingScreen;
    public GameObject pauseMenu;

    [Header("Sliders Components")]
    public Slider volumeSlider;
    public Slider sfxSlider;
    public Slider musicSlider;

    private AudioSource gameplayBgm;
    private AudioSource gameplaySfx;

    void Start()
    {
        // 🎧 Ambil semua AudioSource yang nempel di objek ini
        AudioSource[] sources = GetComponents<AudioSource>();
        if (sources.Length >= 2)
        {
            gameplayBgm = sources[0];
            gameplaySfx = sources[1];
        }

        // 💾 1. BACA MEMORI GLOBAL DARI INDUK (MAIN MENU): Paksa status suaranya
        SyncAudioFromPlayerPrefs();

        // 🎛️ 2. SINKRONISASI VISUAL SLIDER: Biar angka slider gak bohong pas dibuka gess
        InitializeSliders();
    }

    private void SyncAudioFromPlayerPrefs()
    {
        bool isMusicMuted = (PlayerPrefs.GetInt("MusicMuted", 0) == 1);
        bool isSfxMuted = (PlayerPrefs.GetInt("SFXMuted", 0) == 1);

        if (gameplayBgm != null) gameplayBgm.mute = isMusicMuted;
        if (gameplaySfx != null) gameplaySfx.mute = isSfxMuted;

        // Bisu-kan semua SFX lain yang bertebaran di scene gameplay gess
        AudioSource[] allSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (AudioSource src in allSources)
        {
            if (src != gameplayBgm)
            {
                src.mute = isSfxMuted;
            }
        }
    }

    private void InitializeSliders()
    {
        // Jika di Main Menu statusnya MUTED (Bisu = 1), maka slider auto setel ke 0 (Habis)
        // Jika tidak muted, setel ke 1 (Suara penuh)
        if (musicSlider != null && gameplayBgm != null)
        {
            musicSlider.value = gameplayBgm.mute ? 0f : 1f;
            // Daftarkan fungsi pemicu perubahan slider secara dinamis lewat kode gess
            musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
        }

        if (sfxSlider != null && gameplaySfx != null)
        {
            sfxSlider.value = gameplaySfx.mute ? 0f : 1f;
            sfxSlider.onValueChanged.AddListener(OnSfxSliderChanged);
        }

        if (volumeSlider != null)
        {
            // Ambil data volume master, default-nya penuh (1)
            volumeSlider.value = AudioListener.volume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
    }

    // ========================================================
    // 🔌 FUNGSI LOGIC KETIKA SLIDER-NYA DIGESER-GESER PLAYER
    // ========================================================

    public void OnMusicSliderChanged(float value)
    {
        if (gameplayBgm == null) return;

        // Kalau slider digeser ke nol, otomatis anggap Mute. Kalau di atas nol, nyalakan suara!
        gameplayBgm.mute = (value <= 0f);

        // Simpan perubahan ini balik ke memori biar pas balik ke Main Menu ikut sinkron!
        PlayerPrefs.SetInt("MusicMuted", gameplayBgm.mute ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void OnSfxSliderChanged(float value)
    {
        if (gameplaySfx == null) return;

        gameplaySfx.mute = (value <= 0f);

        // Simpan perubahan SFX ke memori global
        PlayerPrefs.SetInt("SFXMuted", gameplaySfx.mute ? 1 : 0);
        PlayerPrefs.Save();

        // Paksa semua audio efek di gameplay langsung ikut volume slider gess
        AudioSource[] allSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (AudioSource src in allSources)
        {
            if (src != gameplayBgm)
            {
                src.mute = gameplaySfx.mute;
            }
        }
    }

    public void SetVolume(float value)
    {
        AudioListener.volume = value;
    }

    public void OpenSetting()
    {
        settingScreen.SetActive(true);
        pauseMenu.SetActive(false);
    }

    public void CloseSetting()
    {
        settingScreen.SetActive(false);
        pauseMenu.SetActive(true);
    }
}