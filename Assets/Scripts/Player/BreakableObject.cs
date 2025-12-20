using UnityEngine;

public class BreakableObject : MonoBehaviour, IDamageable // <-- Bak, ayný ehliyeti kullanýyor
{
    [Header("Dayanýklýlýk")]
    [SerializeField] private float maxHealth = 30f; // 2 vuruþta kýrýlsýn (Mermimiz 20 vuruyordu)

    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    // Interface zorunluluðu: Hasar Alma
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        // Konsola yazalým ki çalýþtýðýný görelim
        Debug.Log(gameObject.name + " hasar aldý! Kalan Can: " + currentHealth);

        // Can bittiyse öl
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("KUTU PARÇALANDI!");

        // Buraya ilerde "Particle System" veya "Loot (Can iksiri)" düþürme kodu gelecek.
        // Þimdilik sadece yok olsun.
        Destroy(gameObject);
    }
}