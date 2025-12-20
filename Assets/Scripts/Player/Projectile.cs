using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float moveSpeed;
    private float damage;
    private float lifeTime = 5f; // 5 saniye sonra kimseye çarpmazsa yok olsun

    // Bu fonksiyonu fýrlatýrken çaðýracaðýz (Kurulum)
    public void Initialize(Vector2 direction, float speed, float dmg)
    {
        moveSpeed = speed;
        damage = dmg;

        // Merminin yönünü ayarla (Saða bakan sprite varsayýyoruz)
        transform.right = direction;

        // Temizlik: Sonsuza kadar sahnede kalmasýn
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // Sürekli ileri git (Hangi yöne bakýyorsa o yöne)
        // Unity 6: transform.Translate hala geçerli ve performansý iyidir.
        transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Çarptýðým þeyde "IDamageable" (Caný Yanan) özelliði var mý?
        IDamageable target = other.GetComponent<IDamageable>();

        if (target != null)
        {
            // Kendimize vurmayalým (Player'da da IDamageable var!)
            if (other.CompareTag("Player")) return;

            // Hasar ver
            target.TakeDamage(damage);
            Debug.Log("Vuruldu: " + other.name);

            // Kendini yok et (Patlama efekti eklenebilir)
            Destroy(gameObject);
        }
        else if (other.CompareTag("Wall") || other.CompareTag("Obstacle")) // Duvara çarparsa
        {
            Destroy(gameObject);
        }
    }
}