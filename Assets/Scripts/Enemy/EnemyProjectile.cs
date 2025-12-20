using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public ControlType targetKey;
    public float lockDuration = 2f;
    public float damage = 5f;
    public float speed = 5f;

    [Header("Difficulty Settings")]
    // This attribute creates a slider in the Inspector between 0.0 (0%) and 1.0 (100%)
    [Range(0f, 1f)]
    public float randomLockChance = 0.5f;

    void Update()
    {
        transform.Translate(Vector2.up * (speed * Time.deltaTime));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponent<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(damage);

            if (Random.value <= randomLockChance)
            {
                InputManager.Instance.ApplyLock(targetKey, lockDuration);
            }

            // 2. Chance-based lock for WASD
            // Random.value returns a number between 0.0 and 1.0
            if (Random.value <= randomLockChance)
            {
                ControlType randomWasd = (ControlType)Random.Range(0, 4);
                InputManager.Instance.ApplyLock(randomWasd, lockDuration);
                Debug.Log("Random WASD Lock Triggered!");
            }

            Destroy(gameObject);
        }
    }
}