using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float healPercentage = 0.10f; // 10 percent
    [SerializeField] private AudioClip collectSound;
    [SerializeField] private GameObject collectEffect;

    private Transform playerTransform;
    public float magnetRange = 3f;
    public float magnetSpeed = 8f;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }
    void Update()
    {
        if (playerTransform == null) return;

        float distance = Vector2.Distance(transform.position, playerTransform.position);
        if (distance < magnetRange)
        {
            transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, magnetSpeed * Time.deltaTime);
        }
    
    // Floating animation
    float newY = Mathf.Sin(Time.time * 5f) * 0.1f;
        transform.position += new Vector3(0, newY * Time.deltaTime, 0);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {

       
        if (other.CompareTag("Player"))
        {
            PlayerStats stats = other.GetComponent<PlayerStats>();

            if (stats != null)
            {
                // Calculate 10% of Max Health
                float healAmount = stats.maxHealth * healPercentage;

                // Call the Heal function in PlayerStats
                stats.Heal(healAmount);

                // Visual and Audio feedback
                if (collectSound != null)
                {
                    AudioSource.PlayClipAtPoint(collectSound, transform.position);
                }

                if (collectEffect != null)
                {
                    Instantiate(collectEffect, transform.position, Quaternion.identity);
                }

                // Destroy the health object
                Destroy(gameObject);
            }
        }
    }
}