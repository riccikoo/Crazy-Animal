using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    [Header("Basic Stats")]
    public int health = 100;
    public int maxHealth = 100;
    public int level = 1;
    public int currentExp = 0;
    public int expToNextLevel = 60;
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
    public float attackEnergyCost = 5f; // Biaya energi sekali pukul
    private bool isAttacking = false;

    [Header("UI Reference")]
    public TextMeshProUGUI healthUI;
    public TextMeshProUGUI expUI;
    public TextMeshProUGUI energyUI;
    public TextMeshProUGUI levelUI;
    public UnityEngine.UI.Slider hpSlider;
    public UnityEngine.UI.Slider energySlider;
    public UnityEngine.UI.Slider xpSlider;
    public GameObject floatingTextPrefab;
    public Canvas canvas;
    public LevelUpEffect levelUpEffect;
    [Header("Audio Settings Custom")]
    public AudioClip hurtSFX;
    public AudioClip xpSFX;
    private AudioSource sfxSource;

    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        energy = maxEnergy;
        UpdateUI();
        GameObject managerObj = GameObject.Find("SFX Manager");
        if (managerObj != null)
        {
            sfxSource = managerObj.GetComponent<AudioSource>();
        }
    }

    void Update()
    {
        HandleEnergyRegen();
    }

    public void AddExperience(int amount)
    {
        currentExp += amount;
        if (sfxSource != null && xpSFX != null && !sfxSource.mute)
            {
                sfxSource.PlayOneShot(xpSFX);
            }
        Debug.Log("Exp +" + amount + "Total :" + currentExp);

        while (currentExp >= expToNextLevel)
        {
            LevelUp();
        }

        RegenHPAfterEat();

        UpdateUI();

        if (floatingTextPrefab != null && canvas != null)
        {
            GameObject ft = Instantiate(floatingTextPrefab, canvas.transform);
            ft.GetComponent<FloatingText>().SetText("+" + amount + " XP");
            ft.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -200);
        }
    }

    void LevelUp()
    {
        currentExp -= expToNextLevel;
        level++;
        expToNextLevel = level * 60;

        maxHealth += 20;
        health = maxHealth;
        damage += 10;
        regenRate +=2;

        Debug.Log("Level Up : Level sekarang " + level);

        if (levelUpEffect) levelUpEffect.ShowLevelUp(level);
    }

    void RegenHPAfterEat()
    {
        int regenAmount = Mathf.RoundToInt(maxHealth * 0.25f);
        health += regenAmount;
        if (health > maxHealth) health = maxHealth;

        Debug.Log("HP +" + regenAmount);
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

    public void ManualAttack()
    {
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
        if (healthUI) healthUI.text = health + "/" + maxHealth;
        if (levelUI) levelUI.text = "LVL: " + level;
        //if (expUI) expUI.text = "Exp: " + currentExp + " / " + expToNextLevel;
        if (expUI) expUI.text = currentExp + "/" + expToNextLevel;
        if (energyUI) energyUI.text = Mathf.RoundToInt(energy) + "/" + Mathf.RoundToInt(maxEnergy);


        if (health <= 0) Debug.Log("Player Mati / Game Over!");
        if (xpSlider) { xpSlider.maxValue = expToNextLevel; xpSlider.value = currentExp; }
        if (hpSlider) { hpSlider.maxValue = maxHealth; hpSlider.value = health; }
        if (energySlider) { energySlider.maxValue = maxEnergy; energySlider.value = energy; }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Heal")) { health += 10; Destroy(other.gameObject); }
        // else if (other.CompareTag("Gold")) {  += 5; Destroy(other.gameObject); }
        else if (other.CompareTag("Damage"))
        {
            health -= 10;

            // 🔥 Suara player mengaduh kesakitan!
            if (sfxSource != null && hurtSFX != null && !sfxSource.mute)
            {
                sfxSource.PlayOneShot(hurtSFX);
            }

            Destroy(other.gameObject);
        }
        UpdateUI();
    }

    // Menampilkan radius serangan di Editor (Garis merah)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}