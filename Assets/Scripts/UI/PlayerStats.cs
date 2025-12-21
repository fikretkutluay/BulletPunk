using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class PlayerStats : MonoBehaviour, IDamageable
{
    // ... [Existing Skill and Audio Headers remain the same] ...
    [Header("Skill Bonuses")]
    public float qDamageMultiplier = 1f;
    public float eDamageMultiplier = 1f;
    public float rDamageMultiplier = 1f;

    [Header("Ses Efektleri")]
    public AudioClip xpSound;
    private AudioSource audioSource;

    [Header("Can Ayarlarý")]
    public float maxHealth = 100f;
    public float currentHealth;
    public float healthIncreaseMult = 1.5f;

    [Header("XP Ayarlarý")]
    public float currentXP = 0f;
    public float xpToNextLevel = 100f;
    public int currentLevel = 1;

    [Header("UI Referanslarý")]
    public Slider healthSlider;
    public Slider xpSlider;
    public TextMeshProUGUI levelText;

    [Header("Hit Effects")]
    public List<GameObject> hitEffectPrefabs;
    public float hitEffectRadius = 0.3f;
    public float hitCooldown = 0.2f;
    private float lastHitTime;

    [Header("Visual Feedback")]
    public SpriteRenderer playerSprite;
    public float flashDuration = 0.1f;
    private Material flashMaterial;

   
    

    private Animator animator;
    private GameMenuManager menuManager;
    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        menuManager = FindFirstObjectByType<GameMenuManager>();
        audioSource = GetComponent<AudioSource>();

        

        UpdateUI();
        if (playerSprite != null)
        {
            // Use .material to create a unique instance for the player
            flashMaterial = playerSprite.material;
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        // --- NEW: HIT COOLDOWN & EFFECTS ---
        if (Time.time < lastHitTime + hitCooldown) return;
        lastHitTime = Time.time;

        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null && movement.IsInvincible) return;

        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;

        // --- SPAWN RANDOM HIT EFFECT ---
        SpawnHitEffect();

        // --- VISUAL FLASH ---
        if (playerSprite != null) StartCoroutine(DamageFlashRoutine());

        UpdateUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void SpawnHitEffect()
    {
        if (hitEffectPrefabs != null && hitEffectPrefabs.Count > 0)
        {
            // Pick random prefab
            int randomIndex = Random.Range(0, hitEffectPrefabs.Count);

            // Calculate random offset
            Vector2 randomOffset = Random.insideUnitCircle * hitEffectRadius;
            Vector3 spawnPos = transform.position + (Vector3)randomOffset + new Vector3(0, 0, -1f);

            // Spawn
            GameObject effect = Instantiate(hitEffectPrefabs[randomIndex], spawnPos, Quaternion.Euler(0, 0, Random.Range(0, 360)));
            Destroy(effect, 0.5f); // Cleanup
        }
    }

    private IEnumerator DamageFlashRoutine()
    {
        if (flashMaterial != null)
        {
            // Set the shader property (ensure name matches your shader)
            flashMaterial.SetFloat("_FlashAmount", 0.941f);

            yield return new WaitForSeconds(flashDuration);

            // Reset to normal
            flashMaterial.SetFloat("_FlashAmount", 0f);
        }
    }

    // ... [Heal, GainXP, LevelUp, and UpdateUI remain the same] ...

    public void Heal(float amount)
    {
        if (isDead) return;
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        UpdateUI();
    }

    public void GainXP(float amount)
    {
        if (isDead) return;

        if (audioSource != null && xpSound != null)
        {
            audioSource.PlayOneShot(xpSound, 0.5f);
        }

        currentXP += amount;
        while (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }
        UpdateUI();
    }

    private void LevelUp()
    {
        currentXP -= xpToNextLevel;
        currentLevel++;
        xpToNextLevel *= 1.2f;

        if (LevelUpManager.Instance != null)
            LevelUpManager.Instance.ShowLevelUpMenu();
        maxHealth *= healthIncreaseMult;
        currentHealth = maxHealth;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (healthSlider != null) { healthSlider.maxValue = maxHealth; healthSlider.value = currentHealth; }
        if (xpSlider != null) { xpSlider.maxValue = xpToNextLevel; xpSlider.value = currentXP; }
        if (levelText != null) levelText.text = "LVL " + currentLevel;
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        Debug.Log("OYUNCU ÖLDÜ - HERKES DURSUN!");

        if (GetComponent<PlayerMovement>()) GetComponent<PlayerMovement>().enabled = false;
        if (GetComponent<PlayerSkillController>()) GetComponent<PlayerSkillController>().enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        if (GetComponent<Collider2D>()) GetComponent<Collider2D>().enabled = false;

        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        foreach (Enemy enemy in enemies)
        {
            enemy.enabled = false;
            if (enemy.GetComponent<Animator>()) enemy.GetComponent<Animator>().enabled = false;
            if (enemy.GetComponent<Rigidbody2D>()) enemy.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        yield return new WaitForSeconds(3f);

        if (menuManager != null)
        {
            menuManager.ShowGameOver();
        }
        else
        {
            Time.timeScale = 0;
        }
    }
}