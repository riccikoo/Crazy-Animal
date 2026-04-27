using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyState { Patrol, Chase, Eat }
    public EnemyState currentState = EnemyState.Patrol;

    [Header("Detection & Combat")]
    public float detectionRadius = 8f;
    public float eatDistance = 1.5f;
    public int damageAmount = 10;
    public int enemyHealth = 100;
    public Transform player;

    [Header("Attack Settings")]
    public float attackCooldown = 10.0f; // Jeda antar gigitan
    private bool canAttack = true;

    [Header("Movement")]
    public float patrolRadius = 10f;
    public float walkSpeed = 2f;
    public float runSpeed = 5f;

    private NavMeshAgent agent;
    private Animator anim;
    private Vector3 spawnPoint;
    private bool isInvincible = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        spawnPoint = transform.position;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        SetNewPatrolTarget();
    }

    void Update()
    {
        // KUNCI 1: Jika sedang makan atau sedang kena hit, JANGAN lakukan logika apa pun
        if (currentState == EnemyState.Eat || isInvincible) return;

        float distance = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case EnemyState.Patrol:
                agent.speed = walkSpeed;
                anim.SetFloat("Speed", 1f);
                if (distance < detectionRadius) currentState = EnemyState.Chase;
                if (!agent.pathPending && agent.remainingDistance < 0.5f) SetNewPatrolTarget();
                break;

            case EnemyState.Chase:
                agent.speed = runSpeed;
                agent.SetDestination(player.position);
                anim.SetFloat("Speed", 2f);
                
                if (distance > detectionRadius + 2f) currentState = EnemyState.Patrol;

                // KUNCI 2: Hanya boleh masuk ke StartEating JIKA canAttack bernilai TRUE
                // dan pastikan state belum dalam posisi Eat
                if (distance <= eatDistance && canAttack && currentState != EnemyState.Eat) 
                {
                    StartCoroutine(StartEating());
                }
                break;
        }
    }

    IEnumerator StartEating()
    {
        // KUNCI 3: Segera matikan izin attack di baris pertama Coroutine
        canAttack = false; 
        currentState = EnemyState.Eat;
        agent.isStopped = true;
        agent.velocity = Vector3.zero; // Paksa berhenti total agar tidak "sliding" sambil nyerang

        // Animasi
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        if (anim != null) anim.Play("eat", -1, 0f);

        // Tunggu momen gigitan (Damage)
        yield return new WaitForSeconds(1.0f); 

        // Cek ulang jarak sebelum kasih damage (siapa tahu player sudah kabur)
        if (Vector3.Distance(transform.position, player.position) <= eatDistance + 0.5f)
        {
            PlayerStats ps = player.GetComponent<PlayerStats>();
            if (ps != null)
            {
                ps.health -= damageAmount;
                ps.UpdateUI();
                Debug.Log("Musuh berhasil menggigit!");
            }
        }

        // Tunggu sisa animasi selesai
        yield return new WaitForSeconds(1.0f); 

        // KUNCI 4: Kembalikan ke Patrol dulu agar AI punya jeda "berpikir" 
        // sebelum mendeteksi player lagi untuk mengejar.
        if (enemyHealth > 0)
        {
            agent.isStopped = false;
            currentState = EnemyState.Patrol;

            // KUNCI 5: Berikan jeda Cooldown yang nyata sebelum canAttack jadi true lagi
            // Selama attackCooldown ini, musuh mungkin ngejar kamu, tapi dia GAK BISA gigit.
            yield return new WaitForSeconds(attackCooldown);
            canAttack = true; 
        }
        else Die();
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible || enemyHealth <= 0) return;
        
        enemyHealth -= damage;
        Debug.Log("Musuh Kena Hit! Sisa Darah: " + enemyHealth);

        if (enemyHealth <= 0) 
        {
            Die();
        }
        else 
        {
            StopCoroutine(StartEating()); // Hentikan makan kalau dipukul
            StartCoroutine(HitCooldown());
        }
    }

    IEnumerator HitCooldown()
    {
        isInvincible = true;
        agent.isStopped = true;
        if (anim != null) anim.Play("eat"); 
        
        yield return new WaitForSeconds(0.2f);

        if (enemyHealth > 0)
        {
            isInvincible = false;
            agent.isStopped = false;
            currentState = EnemyState.Patrol;
            canAttack = true; // Reset canAttack biar bisa menyerang balik
        }
    }

    // --- INI FUNGSI DIE YANG TADI HAMPIR HILANG ---
    void Die()
    {
        Debug.Log("Musuh Mati!");
        StopAllCoroutines(); // Bersihkan semua proses sisa
        
        // Panggil respawn ke spawner (Pastikan script EnemySpawner sudah ada di scene)
        if (EnemySpawner.instance != null)
        {
            EnemySpawner.instance.RespawnEnemy(spawnPoint);
        }
        
        Destroy(gameObject);
    }

    void SetNewPatrolTarget()
    {
        Vector3 rd = Random.insideUnitSphere * patrolRadius;
        rd += spawnPoint;
        NavMeshHit hit;
        NavMesh.SamplePosition(rd, out hit, patrolRadius, 1);
        agent.SetDestination(hit.position);
    }
}