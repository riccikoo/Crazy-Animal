using UnityEngine;
using UnityEngine.UI;

public class AudioToggleSettings : MonoBehaviour
{
    [Header("Audio Components")]
    public AudioSource musicSource;
    public AudioSource sfxSource;       // Ini Speaker SFX kamu

    [Header("Toggle Buttons (Paw)")]
    public Button musicButton;
    public Button sfxButton;

    [Header("Sprites ON/OFF (Canva)")]
    public Sprite spriteOn;
    public Sprite spriteOff;

    [Header("UI Click Asset")]
    public AudioClip uiClickSFX;        // 🔥 Slot buat file audio klik kamu

    private bool isMusicOn = true;
    private bool isSfxOn = true;

    void Start()
    {
        // Daftarkan fungsi klik tombol bawaan
        musicButton.onClick.AddListener(ToggleMusic);
        sfxButton.onClick.AddListener(ToggleSFX);

        musicButton.interactable = true;
        sfxButton.interactable = true;

        // 🔥 JALUR NINJA: Otomatis pasang suara klik ke SEMUA tombol yang ada di menu ini!
        Button[] allButtons = GetComponentsInChildren<Button>(true);
        foreach (Button btn in allButtons)
        {
            btn.onClick.AddListener(PlayClickSound);
        }
    }

    void OnEnable()
    {
        isMusicOn = true;
        isSfxOn = true;

        if (musicSource != null) musicSource.mute = false;
        if (sfxSource != null) sfxSource.mute = false;

        UpdateButtonVisual(musicButton, isMusicOn);
        UpdateButtonVisual(sfxButton, isSfxOn);
    }

    // 🔥 Fungsi buat bunyin suara klik
    void PlayClickSound()
    {
        if (sfxSource != null && uiClickSFX != null)
        {
            sfxSource.PlayOneShot(uiClickSFX);
        }
    }

    void ToggleMusic()
    {
        isMusicOn = !isMusicOn;
        if (musicSource != null) musicSource.mute = !isMusicOn;
        UpdateButtonVisual(musicButton, isMusicOn);
    }

    void ToggleSFX()
    {
        isSfxOn = !isSfxOn;
        if (sfxSource != null) sfxSource.mute = !isSfxOn;
        UpdateButtonVisual(sfxButton, isSfxOn);
    }

    void UpdateButtonVisual(Button targetButton, bool isOn)
    {
        Image buttonImage = targetButton.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.sprite = isOn ? spriteOn : spriteOff;
        }
    }
}