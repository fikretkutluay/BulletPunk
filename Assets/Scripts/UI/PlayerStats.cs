using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PlayerStats : MonoBehaviour, IDamageable
{
    [Header("Skill Bonuses")]
    public float qDamageMultiplier = 1f;
    public float eDamageMultiplier = 1f;
    public float rDamageMultiplier = 1f;
    [Header("Ses Efektleri")] // --- 1. DEÐÝÞKENLERÝ EKLE ---
    public AudioClip xpSound; // XP sesi buraya
    private AudioSource audioSource; // Hoparlör
    [Header("Can Ayarlarý")]
    public float maxHealth = 100f;
    public float currentHealth;
    public float healthIncreaseMult =1.5f;

    [Header("XP Ayarlarý")]
    public float currentXP = 0f;
    public float xpToNextLevel = 100f;
    public int currentLevel = 1;

    [Header("UI Referanslarý")]
    public Slider healthSlider;
    public Slider xpSlider;
    public TextMeshProUGUI levelText;

    // --- ÖLÜM SÝSTEMÝ ---
    private Animator animator;
    private GameMenuManager menuManager;
    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        // Unity 6 için FindFirstObjectByType
        menuManager = FindFirstObjectByType<GameMenuManager>();
        audioSource = GetComponent<AudioSource>();

        UpdateUI();
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null && movement.IsInvincible)
        {
            return;
        }

        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;

        UpdateUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

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

        // --- 3. SESÝ ÇAL ---
        // Her XP geldiðinde çalmasý için buraya ekliyoruz
        if (audioSource != null && xpSound != null)
        {
            // Sesi biraz kýsýk çalalým (0.5f), çok sýk tekrar edebilir
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

        // 1. OYUNCUYU KÝLÝTLE
        if (GetComponent<PlayerMovement>()) GetComponent<PlayerMovement>().enabled = false;
        if (GetComponent<PlayerSkillController>()) GetComponent<PlayerSkillController>().enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        if (GetComponent<Collider2D>()) GetComponent<Collider2D>().enabled = false;

        // 2. TÜM DÜÞMANLARI BUL VE DONDUR (YENÝ KISIM)
        // Sahnedeki tüm "Enemy" scriptlerini buluyoruz
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        foreach (Enemy enemy in enemies)
        {
            // Düþmanýn beynini (Update) kapat
            enemy.enabled = false;

            // Varsa animasyonunu dondur
            if (enemy.GetComponent<Animator>()) enemy.GetComponent<Animator>().enabled = false;

            // Fiziksel olarak kaymasýný engelle
            if (enemy.GetComponent<Rigidbody2D>()) enemy.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }

        // 3. ANÝMASYONU OYNAT
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        // 4. BEKLEME (1.5 saniye - Animasyon bitene kadar)
        yield return new WaitForSeconds(3f);

        // 5. GAME OVER PANELÝ
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