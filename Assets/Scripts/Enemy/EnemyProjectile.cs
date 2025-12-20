using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public ControlType targetKey;
    public float lockDuration = 2f;
    public float damage = 5f;
    public float speed = 5f;

    void Update()
    {
        transform.Translate(Vector2.up * (speed * Time.deltaTime));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Try to damage player
        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
            
            // Apply the specific key lock
            InputManager.Instance.ApplyLock(targetKey, lockDuration);
            
            // 50% chance to also disable a random WASD key
            if (Random.value > 0.5f)
            {
                ControlType randomWasd = (ControlType)Random.Range(0, 4); // MoveUp to MoveRight
                InputManager.Instance.ApplyLock(randomWasd, lockDuration);
            }
            
            Destroy(gameObject);
        }
    }
}