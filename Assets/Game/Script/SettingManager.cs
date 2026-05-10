using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    public GameObject settingScreen;
    public GameObject pauseMenu;
    public Slider volumeSlider;
    public Slider sfxSlider;
    public Slider musicSlider;

    void Start()
    {
        volumeSlider.value = AudioListener.volume;
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

    public void SetVolume(float value)
    {
        AudioListener.volume = value;
    }
}