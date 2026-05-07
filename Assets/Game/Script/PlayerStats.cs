using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    [Header("Basic Stats")]
    public int health = 100;
    public int gold = 0;
    public int damage = 50;

    [Header("Energy System")]
    public float energy = 100f;
    public float maxEnergy = 100f;
    public float regenRate = 5f;
    public float regenDelay = 5f;
    public bool isExhausted = false;
    private float regenTimer = 0f;

    [Header("Attack Settings")]
    public float attackRange = 2f;      // Jarak serangan
    public float attackEnergyCost = 10f; // Biaya energi sekali pukul
    private bool isAttacking = false;

    [Header("UI Reference")]
    public TextMeshProUGUI healthUI;
    public TextMeshProUGUI goldUI;
    public TextMeshProUGUI energyUI;
    public UnityEngine.UI.Slider hpSlider;
    public UnityEngine.UI.Slider energySlider;

    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        energy = maxEnergy;
        UpdateUI();
    }

    void Update()
    {
        HandleEnergyRegen();
    }

    private void HandleEnergyRegen()
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

    // --- LOGIKA ATTACK MANUAL ---
    public void ManualAttack()
    {
        // Hanya bisa nyerang jika tidak sedang animasi attack & tidak capek
        if (!isAttacking && !isExhausted && energy >= attackEnergyCost)
        {
            StartCoroutine(PlayerAttackRoutine());
        }
    }

    IEnumerator PlayerAttackRoutine()
    {
        // 1. Izinkan animasi dimainkan ulang dari awal meskipun animasi sebelumnya belum beres
        if (anim != null)
        {
            // Parameter: "NamaState", Layer (-1 = default), StartTime (0f = mulai dari awal)
            anim.Play("eat", -1, 0f);
        }

        isAttacking = true;
        energy -= attackEnergyCost;
        UpdateUI();

        // 2. Momen serangan (Disesuaikan agar terasa cepat, misal 0.3 detik)
        yield return new WaitForSeconds(0.3f);

        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, attackRange);
        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                EnemyAI enemyScript = enemy.GetComponent<EnemyAI>();
                if (enemyScript != null) enemyScript.TakeDamage(damage);
            }
        }

        // 3. KUNCI UTAMA SPAM:
        // Jangan tunggu sampai 1.2 detik. Begitu damage keluar, langsung izinkan attack lagi.
        yield return new WaitForSeconds(0.1f);
        isAttacking = false;
    }

    // --- UTILITY & UI ---
    public void UseEnergy(float amountPerSecond)
    {
        energy -= amountPerSecond * Time.deltaTime;
        if (energy <= 0)
        {
            energy = 0;
            isExhausted = true;
            regenTimer = regenDelay;
        }
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (healthUI) healthUI.text = "HP: " + health;
        if (goldUI) goldUI.text = "Gold: " + gold;
        if (energyUI) energyUI.text = "Energy: " + Mathf.RoundToInt(energy);

        if (hpSlider) hpSlider.value = health;
        if (energySlider) energySlider.value = energy;

        if (health <= 0) Debug.Log("Player Mati / Game Over!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Heal")) { health += 10; Destroy(other.gameObject); }
        else if (other.CompareTag("Gold")) { gold += 5; Destroy(other.gameObject); }
        else if (other.CompareTag("Damage")) { health -= 10; Destroy(other.gameObject); }
        UpdateUI();
    }

    // Menampilkan radius serangan di Editor (Garis merah)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}