using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyHUDController : MonoBehaviour
{
    public TextMeshProUGUI enemyNameText;
    public Slider enemyHPBar;

    private Transform target;
    private Canvas canvas;
    private RectTransform rectTransform;
    private Camera mainCam;
    private int maxHP;

    public void Init(Transform enemyTransform, string enemyName, int hp)
    {
        target = enemyTransform;
        maxHP = hp;
        mainCam = Camera.main;
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();

        if (enemyNameText) enemyNameText.text = enemyName;
        if (enemyHPBar) { enemyHPBar.maxValue = hp; enemyHPBar.value = hp; }
    }

    public void UpdateHP(int currentHP)
    {
        if (enemyHPBar) enemyHPBar.value = currentHP;
    }

    void LateUpdate()
    {
        if (target == null) { Destroy(gameObject); return; }

        Vector3 screenPos = mainCam.WorldToScreenPoint(target.position + Vector3.up * 3f);
        rectTransform.position = screenPos;
    }
}