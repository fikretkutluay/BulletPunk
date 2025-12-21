using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public enum EnemyType { Fire, Electric, Toxic, Ice }

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("Settings")]
    public EnemyType type;
    public float health = 50f;
    public float moveSpeed = 2f;
    public float stoppingDistance = 5f;
    public float shootInterval = 2f;
    private float lastHitTime;
    private float hitCooldown = 0.05f;

    [Header("Attack Mode")]
    public bool isShotgun = false;
    public int shotgunPelletCount = 3;
    public float shotgunSpreadAngle = 15f;

    [Header("References")]
    public GameObject projectilePrefab;
    private Transform player;
    private float shootTimer;

    [Header("Visual Effects")]
    public float flashDuration = 0.3f;
    private Material flashMaterial;
    private SpriteRenderer spriteRenderer; 
    public float hitEffectRadius = 0.5f;
    public List<GameObject> hitEffectPrefabs = new List<GameObject>();

    [Header("Drops & UI")]
    public GameObject expPrefab;
    public GameObject healthBarPrefab;
    private Slider healthSlider;
    [SerializeField] private float healthBarOffsetY = 0.6f;

    [Header("Knockback Settings")]
    public float knockbackForce = 5f;
    public float knockbackDuration = 0.2f;
    private bool isKnockedBack = false;
    private Rigidbody2D rb;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // .material creates a unique instance for this enemy so flashing doesn't affect all enemies
        flashMaterial = spriteRenderer.material;
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        SetupHealthBar();
    }

    private void SetupHealthBar()
    {
        if (healthBarPrefab != null)
        {
            GameObject hb = Instantiate(healthBarPrefab, transform.position + Vector3.up * healthBarOffsetY, Quaternion.identity, transform);
            healthSlider = hb.GetComponentInChildren<Slider>();

            if (healthSlider != null)
            {
                healthSlider.maxValue = health;
                healthSlider.value = health;
            }
        }
    }

    private void FixedUpdate()
    {
        if (player == null || isKnockedBack) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer > stoppingDistance)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void Update()
    {
        if (player == null) return;

        HandleSpriteFlip();

        shootTimer += Time.deltaTime;
        if (shootTimer >= shootInterval)
        {
            Shoot();
            shootTimer = 0;
        }

        // Keeps the UI from flipping if you decide to flip the sprite
        if (healthSlider != null)
            healthSlider.transform.parent.rotation = Quaternion.identity;
    }

    void HandleSpriteFlip()
    {
        if (player.position.x > transform.position.x)
        {
            spriteRenderer.flipX = true;
        }
        else if (player.position.x < transform.position.x)
        {
            spriteRenderer.flipX = false;
        }
    }

    void Shoot()
    {
        if (projectilePrefab == null) return;

        Vector2 baseDir = (player.position - transform.position).normalized;
        float baseAngle = Mathf.Atan2(baseDir.y, baseDir.x) * Mathf.Rad2Deg - 90f;

        if (isShotgun)
        {
            // Calculate start angle to center the fan on the player
            float startAngle = baseAngle - (shotgunSpreadAngle * (shotgunPelletCount - 1)) / 2f;

            for (int i = 0; i < shotgunPelletCount; i++)
            {
                float currentAngle = startAngle + (i * shotgunSpreadAngle);
                SpawnProjectile(currentAngle);
            }
        }
        else
        {
            SpawnProjectile(baseAngle);
        }
    }

    void SpawnProjectile(float angle)
    {
        GameObject projGO = Instantiate(projectilePrefab, transform.position, Quaternion.Euler(0, 0, angle));
        EnemyProjectile proj = projGO.GetComponent<EnemyProjectile>();

        if (proj != null)
        {
            switch (type)
            {
                case EnemyType.Fire: proj.targetKey = ControlType.Skill1; break;
                case EnemyType.Electric: proj.targetKey = ControlType.Skill2; break;
                case EnemyType.Toxic: proj.targetKey = ControlType.Skill1; break; // Assumed Skill1
                case EnemyType.Ice: proj.targetKey = ControlType.Dash; break;
            }
        }
    }

    public void TakeDamage(float amount)
    {
        // 1. Cooldown check
        if (Time.time < lastHitTime + hitCooldown) return;
        lastHitTime = Time.time;

        health -= amount;
        if (healthSlider != null) healthSlider.value = health;

        // --- RANDOMIZED HIT EFFECT FROM LIST ---
        if (hitEffectPrefabs != null && hitEffectPrefabs.Count > 0)
        {
            // 1. Pick a random prefab from the list
            int randomIndex = Random.Range(0, hitEffectPrefabs.Count);
            GameObject selectedPrefab = hitEffectPrefabs[randomIndex];

            if (selectedPrefab != null)
            {
                // 2. Calculate random spawn position
                Vector2 randomOffset = Random.insideUnitCircle * hitEffectRadius;
                Vector3 spawnPos = new Vector3(
                    transform.position.x + randomOffset.x,
                    transform.position.y + randomOffset.y,
                    -1f
                );

                // 3. Spawn the selected effect with random rotation
                GameObject effect = Instantiate(selectedPrefab, spawnPos, Quaternion.Euler(0, 0, Random.Range(0, 360)));

                // 4. Cleanup
                Destroy(effect, 1f);
            }
        }

        // 3. Apply Knockback
        if (player != null)
        {
            Vector2 knockbackDir = (transform.position - player.position).normalized;
            StartCoroutine(KnockbackRoutine(knockbackDir));
        }

        // 4. Visual Flash
        StartCoroutine(DamageFlashRoutine());

        // --- I REMOVED THE SECOND SPAWN BLOCK THAT WAS HERE ---

        // 5. Death Logic
        if (health <= 0)
        {
            if (expPrefab != null) Instantiate(expPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Set the color of the gizmo
        Gizmos.color = Color.red;

        // Draw a wire circle representing the hit effect radius
        // We use transform.position as the center and hitEffectRadius as the radius
        Gizmos.DrawWireSphere(transform.position, hitEffectRadius);
    }

    private IEnumerator DamageFlashRoutine()
    {
        flashMaterial.SetFloat("_FlashAmount", 0.259f);
        yield return new WaitForSeconds(flashDuration);
        flashMaterial.SetFloat("_FlashAmount", 0f);
    }

    private IEnumerator KnockbackRoutine(Vector2 direction)
    {
        isKnockedBack = true;
        rb.linearVelocity = direction * knockbackForce;

        yield return new WaitForSeconds(knockbackDuration);

        isKnockedBack = false;
        rb.linearVelocity = Vector2.zero;
    }
}