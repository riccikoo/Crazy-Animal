using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner instance;
    public GameObject enemyPrefab;
    public float respawnTime = 20f;

    void Awake() { instance = this; }

    public void RespawnEnemy(Vector3 position)
    {
        StartCoroutine(SpawnRoutine(position));
    }

    IEnumerator SpawnRoutine(Vector3 position)
    {
        Debug.Log("Enemy habis, nunggu 20 detik...");
        yield return new WaitForSeconds(respawnTime);
        Instantiate(enemyPrefab, position, Quaternion.identity);
    }
}