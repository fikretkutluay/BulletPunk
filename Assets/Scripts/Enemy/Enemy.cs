using UnityEngine;
using System.Collections; 
public enum EnemyType { Fire, Electric, Toxic, Ice }

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("Settings")]
    public EnemyType type;
    public float health = 50f;
    public float moveSpeed = 2f;
    public float stoppingDistance = 5f; // <--- NEW: Arrange this in Inspector
    public float shootInterval = 2f;
    
    [Header("References")]
    public GameObject projectilePrefab;
    private Transform player;
    private float shootTimer;
    
    [Header("Visual Effects")]
    public float flashDuration = 0.1f;
    public Material flashMaterial;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    public GameObject hitEffectPrefab;
    
    [Header("Drops")]
    public GameObject expPrefab;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // We access the material instance so we don't change ALL enemies at once
        flashMaterial = spriteRenderer.material; 
    }

    private void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    private void Update()
    {
        if (player == null) return;

        // Calculate distance to player
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // 1. Follow Player (Only if further than stoppingDistance)
        if (distanceToPlayer > stoppingDistance)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            transform.Translate(direction * (moveSpeed * Time.deltaTime));
        }
        // Optional: If you want them to back away if the player gets too close, 
        // you could add an 'else if (distanceToPlayer < stoppingDistance - 1f)' block here.

        // 2. Shooting Logic (Stays the same)
        shootTimer += Time.deltaTime;
        if (shootTimer >= shootInterval)
        {
            Shoot();
            shootTimer = 0;
        }
    }

    void Shoot()
    {
        if (projectilePrefab == null) return;

        GameObject projGO = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        EnemyProjectile proj = projGO.GetComponent<EnemyProjectile>();

        Vector2 dir = player.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        projGO.transform.rotation = Quaternion.Euler(0, 0, angle);

        switch (type)
        {
            case EnemyType.Fire:     proj.targetKey = ControlType.Skill1; break; // Q
            case EnemyType.Electric: proj.targetKey = ControlType.Skill2; break; // E
            case EnemyType.Toxic:    proj.targetKey = ControlType.Skill1; break; // Q (as requested)
            case EnemyType.Ice:      proj.targetKey = ControlType.Dash;   break; // Space
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        StartCoroutine(DamageFlashRoutine());

        if (hitEffectPrefab != null)
        {
            GameObject effect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            
            // Color the particles based on type
            ParticleSystem ps = effect.GetComponent<ParticleSystem>();
            if(ps != null)
            {
                var main = ps.main;
                switch(type) {
                    case EnemyType.Fire: main.startColor = Color.red; break;
                    case EnemyType.Ice: main.startColor = Color.cyan; break;
                    case EnemyType.Toxic: main.startColor = Color.green; break;
                    case EnemyType.Electric: main.startColor = Color.yellow; break;
                }
            }
            Destroy(effect, 1f);
        }

        if (health <= 0) 
        {
            SpawnExperience();
            Destroy(gameObject);
        }
    }

    void SpawnExperience()
    {
        if (expPrefab != null)
        {
            Instantiate(expPrefab, transform.position, Quaternion.identity);
        }
    }

    private IEnumerator DamageFlashRoutine()
    {
        // 1. Turn the flash ON
        // Note: "_FlashAmount" must match the variable name in your Shader
        flashMaterial.SetFloat("_FlashAmount", 0.259f); 

        // 2. Wait for a split second
        yield return new WaitForSeconds(flashDuration);

        // 3. Turn the flash OFF
        flashMaterial.SetFloat("_FlashAmount", 0f);
    }
}