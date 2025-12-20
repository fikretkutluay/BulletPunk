using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class EnemySpawnData
{
    public GameObject prefab;
    [Range(0, 100)] public float spawnWeight; 
    
    [Header("Time Settings (Seconds)")]
    public float minTime; // When this enemy starts appearing
    public float maxTime = 9999f; // When this enemy stops appearing
}

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Area")]
    public float spawnRadius = 10f;
    private Transform player;

    [Header("Timing")]
    public float initialSpawnDelay = 2f;
    public float currentSpawnInterval = 3f;
    public float minSpawnInterval = 0.5f;
    public float difficultyIncreaseRate = 0.05f;
    public float difficultyTickRate = 5f;

    [Header("Enemy Pool")]
    public List<EnemySpawnData> enemyPool = new List<EnemySpawnData>();

    private float spawnTimer;
    private float difficultyTimer;
    private float gameTime; // Tracks total time passed

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
        
        spawnTimer = initialSpawnDelay;
        gameTime = 0f;
    }

    void Update()
    {
        if (player == null) return;

        gameTime += Time.deltaTime; // Track game clock

        // 1. Handle Spawning
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0)
        {
            SpawnEnemy();
            spawnTimer = currentSpawnInterval;
        }

        // 2. Handle Difficulty Increase
        difficultyTimer += Time.deltaTime;
        if (difficultyTimer >= difficultyTickRate)
        {
            IncreaseDifficulty();
            difficultyTimer = 0;
        }
    }

    void SpawnEnemy()
    {
        if (enemyPool.Count == 0) return;

        Vector2 spawnPos = (Vector2)player.position + Random.insideUnitCircle.normalized * spawnRadius;

        // Pick enemy based on weights AND time
        GameObject enemyToSpawn = GetRandomEnemyByWeight();
        
        if (enemyToSpawn != null)
        {
            Instantiate(enemyToSpawn, spawnPos, Quaternion.identity);
        }
    }

    GameObject GetRandomEnemyByWeight()
    {
        // Create a temporary list of enemies allowed at this current time
        List<EnemySpawnData> availableEnemies = new List<EnemySpawnData>();
        float totalWeight = 0;

        foreach (var data in enemyPool)
        {
            // Check if current game time is within the enemy's allowed range
            if (gameTime >= data.minTime && gameTime <= data.maxTime)
            {
                availableEnemies.Add(data);
                totalWeight += data.spawnWeight;
            }
        }

        if (availableEnemies.Count == 0) return null;

        float randomValue = Random.Range(0, totalWeight);
        float currentWeightSum = 0;

        foreach (var data in availableEnemies)
        {
            currentWeightSum += data.spawnWeight;
            if (randomValue <= currentWeightSum)
            {
                return data.prefab;
            }
        }
        return null;
    }

    void IncreaseDifficulty()
    {
        if (currentSpawnInterval > minSpawnInterval)
        {
            currentSpawnInterval -= difficultyIncreaseRate;
        }
    }

    private void OnDrawGizmos()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.position, spawnRadius);
#if UNITY_EDITOR
            UnityEditor.Handles.Label(player.position + Vector3.up * spawnRadius, $"Time: {(int)gameTime}s | Radius: {spawnRadius}");
#endif
        }
    }
}