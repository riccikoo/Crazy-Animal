using UnityEngine;
using UnityEngine.UI;

public class AudioToggleSettings : MonoBehaviour
{
    [Header("Audio Components")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Toggle Buttons (Paw)")]
    public Button musicButton;
    public Button sfxButton;

    [Header("Sprites ON/OFF (Canva)")]
    public Sprite spriteOn;
    public Sprite spriteOff;

    private bool isMusicOn = true;
    private bool isSfxOn = true;

    void Start()
    {
        // Daftarkan fungsi klik tombol
        musicButton.onClick.AddListener(ToggleMusic);
        sfxButton.onClick.AddListener(ToggleSFX);

        // Pastikan tombol 100% selalu AKTIF bisa diklik
        musicButton.interactable = true;
        sfxButton.interactable = true;

        // Setel gambar awal pas game baru dibuka (Mulai dari kondisi ON)
        UpdateButtonVisual(musicButton, isMusicOn);
        UpdateButtonVisual(sfxButton, isSfxOn);
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
        // Ganti gambar secara manual lewat komponen Image tanpa mematikan interactable
        Image buttonImage = targetButton.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.sprite = isOn ? spriteOn : spriteOff;
        }
    }
}