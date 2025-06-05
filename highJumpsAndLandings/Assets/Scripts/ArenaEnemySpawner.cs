using System.Collections.Generic;
using UnityEngine;

public class ArenaEnemySpawner : MonoBehaviour
{
    [SerializeField] private Vector2 spawnArea;
    [SerializeField] private float spawnHeight;
    [SerializeField] private float spawnCollisionCheckRadius;
    [System.Serializable]
    public class EnemySpawn
    {
        public Enemy enemyPrefab;
        public int spawnCost = 2;
    }
    [SerializeField] private EnemySpawn[] enemySpawns;

    [SerializeField] private float spawnInterval;
    private float timeSinceLastSpawn;

    [SerializeField] private int maxEnemyCount;
    private int enemyCount;

    private void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;
        if (timeSinceLastSpawn >= spawnInterval && enemyCount < maxEnemyCount)
        {
            timeSinceLastSpawn = 0;
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        bool validPosition = false;
        Vector3 spawnPosition = Vector3.zero;
        int timesTried = 0;
        while (validPosition == false)
        {
            if (timesTried > 10)
                break;
            timesTried++;

            spawnPosition = new(Random.Range(-spawnArea.x * 0.5f, spawnArea.x * 0.5f), spawnHeight, Random.Range(-spawnArea.y * 0.5f, spawnArea.y * 0.5f));
            validPosition = Physics.CheckSphere(spawnPosition, spawnCollisionCheckRadius) == false;
        }
        Enemy enemyInstance = Instantiate(enemySpawns[0].enemyPrefab, spawnPosition, Quaternion.identity);
        enemyCount++;
        enemyInstance.OnEnemyDeath += EnemyInstance_OnEnemyDeath;
    }

    private void EnemyInstance_OnEnemyDeath(Enemy enemy)
    {
        enemyCount--;
        enemy.OnEnemyDeath -= EnemyInstance_OnEnemyDeath;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(Vector3.up * spawnHeight, new Vector3(spawnArea.x, 0, spawnArea.y));
    }

}
