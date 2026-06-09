using UnityEngine;
using System.Collections;
using UnityEngine.UI; // 🔥 WAJIB ADA INI BIAR IMAGE DI KENAL SAMA UNITY GESS

public class SettingsPopup : MonoBehaviour
{
    public GameObject settingsCanvas;
    public RectTransform settingsPanel;

    public GameObject mainButtons;
    public GameObject selectCharacterButton;
    public GameObject characterHolder;

    [Header("Audio Settings From SceneSoundManager")]
    public AudioSource bgmAudioSource;
    public AudioSource sfxAudioSource;

    [Header("Toggle UI Elements")]
    public Image musicToggleImage; // Drag objek MusicToggle ke sini nanti gess
    public Image sfxToggleImage;   // Drag objek SFXToggle ke sini nanti gess
    public Sprite spriteOn;        // Taruh gambar 'on' kuning di sini
    public Sprite spriteOff;       // Taruh gambar 'off' cokelat di sini

    public Vector2 hiddenPos = new Vector2(0, -900);
    public Vector2 showPos = new Vector2(0, 0);

    public float speed = 8f;

    void Start()
    {
        settingsPanel.anchoredPosition = hiddenPos;
        settingsCanvas.SetActive(false);

        if (characterHolder != null) characterHolder.SetActive(true);

        // 🎧 Cari AudioSource bawaan SceneSoundManager
        GameObject soundManager = GameObject.Find("SceneSoundManager");
        if (soundManager != null)
        {
            AudioSource[] sources = soundManager.GetComponents<AudioSource>();
            if (sources.Length >= 2)
            {
                bgmAudioSource = sources[0]; // BGM (Atas)
                sfxAudioSource = sources[1]; // SFX Click (Bawah)
            }
        }

        // 💾 BACA MEMORI: Ambil status mute dari PlayerPrefs & ganti gambar otomatis pas start
        if (bgmAudioSource != null)
        {
            bgmAudioSource.mute = (PlayerPrefs.GetInt("MusicMuted", 0) == 1);
            if (musicToggleImage != null && spriteOn != null && spriteOff != null)
            {
                // ✅ SUDAH DIPERBAIKI: Music ngurus MusicToggle
                musicToggleImage.sprite = bgmAudioSource.mute ? spriteOff : spriteOn;
            }
        }

        if (sfxAudioSource != null)
        {
            sfxAudioSource.mute = (PlayerPrefs.GetInt("SFXMuted", 0) == 1);
            if (sfxToggleImage != null && spriteOn != null && spriteOff != null)
            {
                // ✅ SUDAH DIPERBAIKI: SFX ngurus SFXToggle
                sfxToggleImage.sprite = sfxAudioSource.mute ? spriteOff : spriteOn;
            }
        }

        SyncGlobalSFX();
    }

    public void ToggleMusic()
    {
        if (bgmAudioSource != null)
        {
            bgmAudioSource.mute = !bgmAudioSource.mute;
            PlayerPrefs.SetInt("MusicMuted", bgmAudioSource.mute ? 1 : 0);
            PlayerPrefs.Save();
            Debug.Log("Music Mute Status: " + bgmAudioSource.mute);

            // ✅ SUDAH DIPERBAIKI TOTAL GESS: Music ngurus MusicToggle
            if (musicToggleImage != null && spriteOn != null && spriteOff != null)
            {
                musicToggleImage.sprite = bgmAudioSource.mute ? spriteOff : spriteOn;
            }
        }
    }

    public void ToggleSFX()
    {
        if (sfxAudioSource != null)
        {
            sfxAudioSource.mute = !sfxAudioSource.mute;
            PlayerPrefs.SetInt("SFXMuted", sfxAudioSource.mute ? 1 : 0);
            PlayerPrefs.Save();
            Debug.Log("SFX Mute Status: " + sfxAudioSource.mute);

            // ✅ SUDAH DIPERBAIKI TOTAL GESS: SFX ngurus SFXToggle
            if (sfxToggleImage != null && spriteOn != null && spriteOff != null)
            {
                sfxToggleImage.sprite = sfxAudioSource.mute ? spriteOff : spriteOn;
            }

            SyncGlobalSFX(); // Suara hewan dll langsung ikut bisu
        }
    }

    private void SyncGlobalSFX()
    {
        if (sfxAudioSource == null) return;

        bool isSfxMuted = sfxAudioSource.mute;

        GameObject animalSoundObj = GameObject.Find("AnimalSound");
        if (animalSoundObj != null)
        {
            AudioSource animalAudio = animalSoundObj.GetComponent<AudioSource>();
            if (animalAudio != null)
            {
                animalAudio.mute = isSfxMuted;
            }
        }

        AudioSource[] allAudioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (AudioSource source in allAudioSources)
        {
            if (source != bgmAudioSource)
            {
                source.mute = isSfxMuted;
            }
        }
    }

    public void OpenSettings()
    {
        settingsCanvas.SetActive(true);
        mainButtons.SetActive(false);
        selectCharacterButton.SetActive(false);

        StopAllCoroutines();
        StartCoroutine(MovePanel(showPos));
        if (characterHolder != null) characterHolder.SetActive(false);
    }

    public void CloseSettings()
    {
        StopAllCoroutines();
        StartCoroutine(CloseRoutine());
    }

    IEnumerator CloseRoutine()
    {
        mainButtons.SetActive(true);
        selectCharacterButton.SetActive(true);

        yield return MovePanel(hiddenPos);

        settingsCanvas.SetActive(false);
        if (characterHolder != null) characterHolder.SetActive(true);
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