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
        
        if (NavMesh.SamplePosition(randomPos, out hit, spawnRadius, NavMesh.AllAreas))
        {
            GameObject newEnemy = Instantiate(enemyPrefab, hit.position, Quaternion.identity);
            
            // Link musuh ke spawner ini
            EnemyAI ai = newEnemy.GetComponent<EnemyAI>();
            if (ai != null) ai.mySpawner = this;

            // Tempelkan ke NavMesh
            NavMeshAgent agent = newEnemy.GetComponent<NavMeshAgent>();
            if (agent != null) agent.Warp(hit.position);
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