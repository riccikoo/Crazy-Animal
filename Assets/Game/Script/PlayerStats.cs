using UnityEngine;
using TMPro;
using System.Collections; // Wajib untuk Coroutine

public class PlayerStats : MonoBehaviour
{
    [Header("Stats")]
    public int health = 100;
    public int gold = 0;
    public float energy = 100f;
    public float maxEnergy = 100f;
    public int damage = 50;

    [Header("Energy Settings")]
    public float regenRate = 5f;
    public float regenDelay = 5f;
    public float regenTimer = 0f;
    public bool isExhausted = false;

    [Header("UI Reference")]
    public TextMeshProUGUI healthUI;
    public TextMeshProUGUI goldUI;
    public TextMeshProUGUI energyUI;

    private Animator anim;
    private bool isAttacking = false; // Biar gak spam damage pas tabrakan

    void Start()
    {
        anim = GetComponent<Animator>();
        energy = maxEnergy;
        UpdateUI();
    }

    void Update()
    {
        if (isExhausted)
        {
            regenTimer -= Time.deltaTime;
            if (regenTimer <= 0) isExhausted = false;
        }
        else if (energy < maxEnergy)
        {
            energy += regenRate * Time.deltaTime;
            energy = Mathf.Clamp(energy, 0, maxEnergy);
            UpdateUI();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Jika nabrak musuh dan kita lagi gak dalam proses nyerang
        if (collision.gameObject.CompareTag("Enemy") && !isAttacking)
        {
            StartCoroutine(PlayerAttackRoutine(collision.gameObject));
        }
    }

    IEnumerator PlayerAttackRoutine(GameObject enemy)
    {
        isAttacking = true; 
        if (anim != null) anim.Play("eat");

        // Tunggu 0.8 detik sampai gerakan animasi "pas" buat ngasih damage
        yield return new WaitForSeconds(0.8f);

        if (enemy != null)
        {
            EnemyAI enemyScript = enemy.GetComponent<EnemyAI>();
            if (enemyScript != null)
            {
                enemyScript.TakeDamage(damage);
            }
        }

        // Tunggu sisa animasi beres baru bisa nyerang lagi
        yield return new WaitForSeconds(1.2f);
        isAttacking = false;
    }

    // Fungsi UI dan Trigger tetap sama
    public void UpdateUI()
    {
        if(healthUI) healthUI.text = "HP: " + health;
        if(goldUI) goldUI.text = "Gold: " + gold;
        if(energyUI) energyUI.text = "Energy: " + Mathf.RoundToInt(energy);
        if (health <= 0) Debug.Log("Game Over!");
    }

    public void UseEnergy(float amount)
    {
        energy -= amount * Time.deltaTime;
        energy = Mathf.Clamp(energy, 0, maxEnergy);
        if (energy <= 0) { isExhausted = true; regenTimer = regenDelay; }
        UpdateUI();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Heal")) { health += 10; Destroy(other.gameObject); }
        else if (other.CompareTag("Gold")) { gold += 5; Destroy(other.gameObject); }
        else if (other.CompareTag("Damage")) { health -= 10; Destroy(other.gameObject); }
        UpdateUI();
    }
}