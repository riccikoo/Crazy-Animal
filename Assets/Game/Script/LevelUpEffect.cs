using UnityEngine;
using TMPro;
using System.Collections;

public class LevelUpEffect : MonoBehaviour
{
    public TextMeshProUGUI levelUpText;
    public float displayDuration = 2f;

    [Header("Audio Settings")]
    public AudioClip levelUpSFX;       // Slot buat file suara Level Up kamu gess
    private AudioSource sfxSource;     // Ini buat pinjem speaker dari SFXManager

    void Start()
    {
        levelUpText = GetComponent<TextMeshProUGUI>();
        levelUpText.enabled = false;

        // 🔥 OTOMATIS: Langsung nyari objek SFXManager di Hierarchy biar kamu gak repot
        GameObject managerObj = GameObject.Find("SFX Manager");
        if (managerObj != null)
        {
            sfxSource = managerObj.GetComponent<AudioSource>();
        }
    }

    public void ShowLevelUp(int newLevel)
    {
        StopAllCoroutines();
        StartCoroutine(Animate(newLevel));
    }

    IEnumerator Animate(int newLevel)
    {
        levelUpText.text = "LEVEL UP!\nLevel " + newLevel;
        levelUpText.enabled = true;

        // 🔥 JALUR NINJA: Pas teks muncul, suara terompet/lonceng langsung joss berbunyi!
        if (sfxSource != null && levelUpSFX != null && !sfxSource.mute)
        {
            sfxSource.PlayOneShot(levelUpSFX);
        }

        float elapsed = 0f;
        Color c = levelUpText.color;

        while (elapsed < displayDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / displayDuration);
            levelUpText.color = new Color(c.r, c.g, c.b, alpha);
            yield return null;
        }

        levelUpText.enabled = false;
    }
}