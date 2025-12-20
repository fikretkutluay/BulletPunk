using UnityEngine;

public class LightningSpell : MonoBehaviour
{
    private float damage;
    [SerializeField]private float hitRadius = 1.5f; // Yýldýrýmýn etki alaný geniþliði
    private float lifeTime = 1.0f;  // Efektin ekranda kalma süresi

    // Bu fonksiyonu Controller'dan çaðýrýp hasarý belirleyeceðiz
    public void Initialize(float dmg, float radius)
    {
        damage = dmg;
        hitRadius = radius;

        // Hasar verme iþlemini hemen baþlat
        Explode();

        // Efekti bir süre sonra yok et
        Destroy(gameObject, lifeTime);
    }

    private void Explode()
    {
        // 1. Belirlenen yarýçapta bir daire çiz ve içindeki her þeyi bul
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, hitRadius);

        foreach (Collider2D obj in hitObjects)
        {
            // Kendimize (Player) vurmayalým
            if (obj.CompareTag("Player")) continue;

            // 2. Çarptýðýmýz þeyde IDamageable interface'i var mý?
            IDamageable target = obj.GetComponent<IDamageable>();

            if (target != null)
            {
                // Hasar ver
                target.TakeDamage(damage);
                Debug.Log(obj.name + " elektriðe çarpýldý!");
            }
        }
    }

    // Editörde etki alanýný görmek için (Gizmos)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position , hitRadius);
    }
}