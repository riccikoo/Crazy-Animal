using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;
    public int maxEnemies = 3;        // Jumlah babi yang ingin di-spawn
    public float spawnRadius = 5f;    // Agar babi tidak numpuk di satu titik saat lahir
    public float respawnTime = 5f;

    void Start()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("[EnemySpawner] enemyPrefab is not assigned in Inspector!");
            return;
        }
        
        Debug.Log("[EnemySpawner] Starting spawn sequence, maxEnemies: " + maxEnemies);
        Debug.Log("[EnemySpawner] Spawner position: " + transform.position);
        
        // Panggil fungsi untuk spawn awal sebanyak maxEnemies
        for (int i = 0; i < maxEnemies; i++)
        {
            SpawnNewEnemy(transform.position);
        }
    }

    // Fungsi internal untuk membuat babi baru
    void SpawnNewEnemy(Vector3 centerPosition)
    {
        // Cari posisi acak biar gak tumpang tindih
        Vector3 randomPos = centerPosition + (Random.insideUnitSphere * spawnRadius);
        NavMeshHit hit;
        
        Debug.Log("[EnemySpawner] Attempting to spawn at randomPos: " + randomPos);
        
        if (NavMesh.SamplePosition(randomPos, out hit, spawnRadius, NavMesh.AllAreas))
        {
            Debug.Log("[EnemySpawner] NavMesh sample SUCCESS at: " + hit.position);
            GameObject newEnemy = Instantiate(enemyPrefab, hit.position, Quaternion.identity);
            
            if (newEnemy == null)
            {
                Debug.LogError("[EnemySpawner] Failed to instantiate enemyPrefab!");
                return;
            }
            
            Debug.Log("[EnemySpawner] Enemy spawned: " + newEnemy.name);
            
            // Link musuh ke spawner ini
            EnemyAI ai = newEnemy.GetComponent<EnemyAI>();
            if (ai != null) 
            {
                ai.mySpawner = this;
                Debug.Log("[EnemySpawner] EnemyAI component linked to spawner");
            }
            else
            {
                Debug.LogError("[EnemySpawner] EnemyAI component not found on spawned enemy!");
            }

            // Tempelkan ke NavMesh
            NavMeshAgent agent = newEnemy.GetComponent<NavMeshAgent>();
            if (agent != null) 
            {
                agent.Warp(hit.position);
                Debug.Log("[EnemySpawner] NavMeshAgent warped to: " + hit.position);
            }
            else
            {
                Debug.LogError("[EnemySpawner] NavMeshAgent component not found!");
            }
        }
        else
        {
            Debug.LogError("[EnemySpawner] NavMesh.SamplePosition FAILED at: " + randomPos);
        }
    }

    // Fungsi yang dipanggil oleh EnemyAI saat mati
    public void RespawnEnemy(Vector3 lastPosition)
    {
        StartCoroutine(RespawnRoutine(lastPosition));
    }

    IEnumerator RespawnRoutine(Vector3 lastPosition)
    {
        yield return new WaitForSeconds(respawnTime);
        SpawnNewEnemy(transform.position); // Spawn babi baru di area spawner
    }
}