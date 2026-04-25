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
        if (currentState == EnemyState.Eat) return;

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
                if (distance <= eatDistance) StartCoroutine(StartEating());
                break;
        }
    }

    IEnumerator StartEating()
    {
        if (currentState == EnemyState.Eat) yield break;
        currentState = EnemyState.Eat;
        agent.isStopped = true;

        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        if (anim != null) anim.Play("eat");

        // --- JEDA DAMAGE (Gantinya Event) ---
        yield return new WaitForSeconds(1.0f); // Tunggu animasi makan 1 detik

        if (Vector3.Distance(transform.position, player.position) <= eatDistance + 1f)
        {
            PlayerStats ps = player.GetComponent<PlayerStats>();
            if (ps != null)
            {
                ps.health -= damageAmount;
                ps.UpdateUI();
            }
        }

        yield return new WaitForSeconds(1.0f); // Selesaikan sisa animasi

        if (enemyHealth > 0)
        {
            agent.isStopped = false;
            currentState = EnemyState.Patrol;
        }
        else Die();
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible || enemyHealth <= 0) return;
        
        enemyHealth -= damage;
        Debug.Log("Musuh Kena Hit! Sisa Darah: " + enemyHealth);

        if (enemyHealth <= 0) Die();
        else StartCoroutine(HitCooldown());
    }

    IEnumerator HitCooldown()
    {
        isInvincible = true;
        // Saat dipukul musuh juga berhenti sebentar
        agent.isStopped = true;
        if (anim != null) anim.Play("eat"); 
        
        yield return new WaitForSeconds(2.0f);

        if (enemyHealth > 0)
        {
            isInvincible = false;
            agent.isStopped = false;
            currentState = EnemyState.Patrol;
        }
    }

    void Die()
    {
        StopAllCoroutines();
        EnemySpawner.instance.RespawnEnemy(spawnPoint);
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