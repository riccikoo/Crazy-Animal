using UnityEngine;
using TMPro;
using System.Collections;

public class HPWarning : MonoBehaviour
{
    public TextMeshProUGUI warningText;
    public PlayerStats playerStats;
    public int hpThreshold = 30;

    private bool isBlinking = false;

    void Update()
    {
        if (playerStats.health <= hpThreshold && !isBlinking)
        {
            isBlinking = true;
            StartCoroutine(Blink());
        }
        else if (playerStats.health > hpThreshold && isBlinking)
        {
            isBlinking = false;
            StopAllCoroutines();
            warningText.enabled = false;
        }
    }

    IEnumerator Blink()
    {
        while (isBlinking)
        {
            warningText.enabled = true;
            yield return new WaitForSeconds(0.4f);
            warningText.enabled = false;
            yield return new WaitForSeconds(0.4f);
        }
    }
}