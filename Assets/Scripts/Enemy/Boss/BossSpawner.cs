using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject bossPrefab;
    public float timeToSpawn = 60f; // Boss spawns after 60 seconds
    public Transform spawnLocation;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip spawnSound;

    private float timer = 0f;
    private bool hasSpawned = false;

    void Update()
    {
        if (hasSpawned) return;

        timer += Time.deltaTime;

        if (timer >= timeToSpawn)
        {
            SpawnBoss();
        }
    }

    void SpawnBoss()
    {
        hasSpawned = true;

        // 1. Play the Sound
        if (audioSource != null && spawnSound != null)
        {
            audioSource.PlayOneShot(spawnSound);
        }

        // 2. Spawn the Dragon
        if (bossPrefab != null)
        {
            Vector2 pos = spawnLocation != null ? spawnLocation.position : Vector2.zero;
            Instantiate(bossPrefab, pos, Quaternion.identity);
        }

        Debug.Log("The Cyber Dragon has arrived!");
    }
}