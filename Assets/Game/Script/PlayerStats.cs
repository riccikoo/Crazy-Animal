using UnityEngine;
using TMPro; // Penting untuk TextMeshPro

public class PlayerStats : MonoBehaviour
{
    public int health = 100;
    public int gold = 0;

    public TextMeshProUGUI healthUI;
    public TextMeshProUGUI goldUI;

    void Start()
    {
        UpdateUI();
    }

    // Fungsi yang otomatis jalan saat nabrak benda "Is Trigger"
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Heal"))
        {
            health += 10;
            Destroy(other.gameObject); // Hilangkan bola setelah diambil
        }
        else if (other.CompareTag("Gold"))
        {
            gold += 5;
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Damage"))
        {
            health -= 10;
            Destroy(other.gameObject);
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        healthUI.text = "HP: " + health;
        goldUI.text = "Gold: " + gold;

        // Cek jika mati
        if (health <= 0)
        {
            Debug.Log("Game Over!");
            // Tambahkan logika mati di sini (misal: reload level)
        }
    }
}