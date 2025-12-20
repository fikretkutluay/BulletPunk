using UnityEngine;
using System.Collections;

public class SwarmSpawner : MonoBehaviour 
{
    public GameObject iceEnemyPrefab;
    public float spawnInterval = 0.15f; 
    public int snakeLength = 15;
    public float swarmCooldown = 30f; 

    private Transform player;

    void Start() {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
        
        StartCoroutine(SwarmRoutine());
    }

    void Update() {
        // --- DEBUG INSTANT SPAWN ---
        if (Input.GetKeyDown(KeyCode.K)) {
            Debug.Log("Manual Swarm Triggered!");
            SpawnSnakeSwarm();
        }
    }

    // This allows you to right-click the component in the Inspector to test
    [ContextMenu("Trigger Swarm Now")]
    public void SpawnSnakeSwarm() {
        if (player == null) return;

        // 1. Pick a starting position just off-screen
        Vector2 spawnOrigin = (Vector2)player.position + Random.insideUnitCircle.normalized * 15f;
        
        // 2. Aim to cross the player's general area
        Vector2 targetPoint = (Vector2)player.position + Random.insideUnitCircle * 5f;
        Vector2 moveDir = (targetPoint - spawnOrigin).normalized;

        StartCoroutine(SpawnSnakeSequence(spawnOrigin, moveDir));
    }

    IEnumerator SpawnSnakeSequence(Vector2 origin, Vector2 direction) {
        for (int i = 0; i < snakeLength; i++) {
            GameObject segment = Instantiate(iceEnemyPrefab, origin, Quaternion.identity);
        
            // Get the projectile prefab from the original script before we destroy it
            Enemy oldEnemyScript = segment.GetComponent<Enemy>();
            GameObject bullet = oldEnemyScript.projectilePrefab;
        
            if(oldEnemyScript != null) Destroy(oldEnemyScript); 

            // Add Swarm logic
            SwarmMember swarmLogic = segment.AddComponent<SwarmMember>();
            swarmLogic.moveDirection = direction;
            swarmLogic.projectilePrefab = bullet; // <--- Pass the bullet here!
            swarmLogic.speed = 8f;
            swarmLogic.frequency = 6f;
            swarmLogic.magnitude = 3f;

            yield return new WaitForSeconds(0.12f); 
        }
    }
    IEnumerator SwarmRoutine() {
        while (true) {
            yield return new WaitForSeconds(swarmCooldown);
            SpawnSnakeSwarm();
        }
    }
}