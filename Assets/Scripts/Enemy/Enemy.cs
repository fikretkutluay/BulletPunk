using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum EnemyType { Fire, Electric, Toxic, Ice }

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("Settings")]
    public EnemyType type;
    public float health = 50f;
    public float moveSpeed = 2f;
    public float stoppingDistance = 5f;
    public float shootInterval = 2f;

    [Header("References")]
    public GameObject projectilePrefab;
    private Transform player;
    private float shootTimer;

    [Header("Visual Effects")]
    public float flashDuration = 0.1f;
    private Material flashMaterial;
    private SpriteRenderer spriteRenderer;
    public GameObject hitEffectPrefab;

    [Header("Drops & UI")]
    public GameObject expPrefab;
    public GameObject healthBarPrefab;
    private Slider healthSlider;

    // Change this value to move the bar up or down
    [SerializeField] private float healthBarOffsetY = 0.6f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Using .material creates a unique instance for this enemy
        flashMaterial = spriteRenderer.material;
    }

    private void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        SetupHealthBar();
    }

    void SetupHealthBar()
    {
        if (healthBarPrefab != null)
        {
            // We spawn it closer (0.6f instead of 1.5f)
            GameObject hb = Instantiate(healthBarPrefab, transform.position + Vector3.up * healthBarOffsetY, Quaternion.identity, transform);
            healthSlider = hb.GetComponentInChildren<Slider>();

            if (healthSlider != null)
            {
                healthSlider.maxValue = health;
                healthSlider.value = health;
            }
        }
    }

    private void Update()
    {
        if (player == null) return;

        // 1. FLIP ÝÞLEMÝNÝ BURADA YAPIYORUZ
        HandleSpriteFlip();

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer > stoppingDistance)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            transform.Translate(direction * moveSpeed * Time.deltaTime);
        }

        shootTimer += Time.deltaTime;
        if (shootTimer >= shootInterval)
        {
            Shoot();
            shootTimer = 0;
        }

        // Keep UI from flipping if you decide to flip the sprite later
        if (healthSlider != null) healthSlider.transform.parent.rotation = Quaternion.identity;
    }

    // YENÝ EKLENEN FONKSÝYON
    void HandleSpriteFlip()
    {
        // Eðer oyuncu düþmanýn saðýndaysa (x deðeri büyükse)
        if (player.position.x > transform.position.x)
        {
            // Sprite saða baksýn (flipX kapalý)
            // NOT: Eðer sprite'ýn orijinali sola bakýyorsa burayý true yap.
            spriteRenderer.flipX = true;
        }
        // Eðer oyuncu düþmanýn solundaysa (x deðeri küçükse)
        else if (player.position.x < transform.position.x)
        {
            // Sprite sola baksýn (flipX açýk)
            spriteRenderer.flipX = false;
        }
    }

    void Shoot()
    {
        if (projectilePrefab == null) return;

        GameObject projGO = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        EnemyProjectile proj = projGO.GetComponent<EnemyProjectile>();

        Vector2 dir = (player.position - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        projGO.transform.rotation = Quaternion.Euler(0, 0, angle);

        switch (type)
        {
            case EnemyType.Fire: proj.targetKey = ControlType.Skill1; break;
            case EnemyType.Electric: proj.targetKey = ControlType.Skill2; break;
            case EnemyType.Toxic: proj.targetKey = ControlType.Skill1; break;
            case EnemyType.Ice: proj.targetKey = ControlType.Dash; break;
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (healthSlider != null) healthSlider.value = health;

        StartCoroutine(DamageFlashRoutine());

        if (hitEffectPrefab != null)
        {
            GameObject effect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            // Optional: Color particles here as we did before
            Destroy(effect, 1f);
        }

        if (health <= 0)
        {
            if (expPrefab != null) Instantiate(expPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    private IEnumerator DamageFlashRoutine()
    {
        flashMaterial.SetFloat("_FlashAmount", 0.259f);
        yield return new WaitForSeconds(flashDuration);
        flashMaterial.SetFloat("_FlashAmount", 0f);
    }
}