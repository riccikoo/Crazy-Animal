using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Unity.VisualScripting;

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
    public EnemySpawner mySpawner;

    [Header("Attack Settings")]
    public float attackCooldown = 10.0f; // Jeda antar gigitan
    private bool canAttack = true;

    [Header("Movement")]
    public float patrolRadius = 10f;
    public float walkSpeed = 2f;
    public float runSpeed = 5f;

    [Header("Level")]
    public int enemyLevel = 1;

    [Header("HUD")]
    public GameObject enemyHUDPrefab;
    private EnemyHUDController hudController;

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

        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (enemyHUDPrefab != null && canvas != null)
        {
            GameObject hud = Instantiate(enemyHUDPrefab, canvas.transform);
            hudController = hud.GetComponent<EnemyHUDController>();
            hudController.Init(transform, gameObject.name, enemyHealth);
        }
    }

    void Update()
    {
        if (currentState == EnemyState.Eat || isInvincible) return;

        float distance = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case EnemyState.Patrol:
                agent.speed = walkSpeed;
                anim.SetFloat("Speed", 1f);

                if (enemyLevel > 0 && distance < detectionRadius)
                {
                    currentState = EnemyState.Chase;
                }

                if (!agent.pathPending && agent.remainingDistance < 0.5f) SetNewPatrolTarget();
                break;

            case EnemyState.Chase:
                if (enemyLevel <= 0)
                {
                    currentState = EnemyState.Patrol;
                    break;
                }

                agent.speed = runSpeed;
                agent.SetDestination(player.position);
                anim.SetFloat("Speed", 2f);

                if (distance > detectionRadius + 2f) currentState = EnemyState.Patrol;

                if (distance <= eatDistance && canAttack && currentState != EnemyState.Eat)
                {
                    StartCoroutine(StartEating());
                }
                break;
        }
    }

    IEnumerator StartEating()
    {
        if(enemyLevel == 0 ) yield break;
        canAttack = false;
        currentState = EnemyState.Eat;
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        if (anim != null) anim.Play("eat", -1, 0f);

        yield return new WaitForSeconds(1.0f);

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

        yield return new WaitForSeconds(1.0f);

        if (enemyHealth > 0)
        {
            agent.isStopped = false;
            currentState = EnemyState.Patrol;
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

        if (hudController) hudController.UpdateHP(enemyHealth);

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

    void Die()
    {
        Debug.Log(gameObject.name + " Mati!");
        StopAllCoroutines();

        // Berikan EXP
        PlayerStats ps = player.GetComponent<PlayerStats>();
        if (ps != null)
        {
            int expReward = (enemyLevel <= 0) ? 20 : enemyLevel * 20;
            ps.AddExperience(expReward);
        }

        // Panggil Respawn lewat spawner spesifiknya
        if (mySpawner != null)
        {
            mySpawner.RespawnEnemy(spawnPoint);
        }
        else
        {
            Debug.LogError("Bunny mati tapi gak punya spawner! Cek Inspector!");
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