using UnityEngine;
using UnityEngine.UI; // UI kullanacaðýz

using TMPro;
public class PlayerStats : MonoBehaviour, IDamageable
{
    [Header("Skill Bonuses")]
    public float qDamageMultiplier = 1f;
    public float eDamageMultiplier = 1f;
    public float rDamageMultiplier = 1f;

    [Header("Can Ayarlarý")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("XP Ayarlarý")]
    public float currentXP = 0f;
    public float xpToNextLevel = 100f;
    public int currentLevel = 1;

    [Header("UI Referanslarý")]
    public Slider healthSlider;
    public Slider xpSlider;
    public TextMeshProUGUI levelText; // Level 1, Level 2 yazýsý için (Opsiyonel)

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }

    // IDamageable'dan gelen hasar alma fonksiyonu
    public void TakeDamage(float amount)
    {
        // Check if the player is currently dashing and invincible
        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null && movement.IsInvincible)
        {
            Debug.Log("Dash used! Damage avoided.");
            return; // Exit the function without taking damage
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
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        UpdateUI();
    }

    public void GainXP(float amount)
    {
        currentXP += amount;

        // Use a 'while' loop instead of 'if' in case the boss gives 
        // enough XP for 2 or 3 levels at once!
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

        // Call the Level Up Menu
        if (LevelUpManager.Instance != null)
        {
            LevelUpManager.Instance.ShowLevelUpMenu();
        }

        currentHealth = maxHealth;
        UpdateUI();

        if (LevelUpManager.Instance != null)
        {
            LevelUpManager.Instance.ShowLevelUpMenu();
        }

    }

    private void UpdateUI()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        if (xpSlider != null)
        {
            xpSlider.maxValue = xpToNextLevel;
            xpSlider.value = currentXP;
        }

        if (levelText != null)
            levelText.text = "LVL " + currentLevel;
    }

    private void Die()
    {
        Debug.Log("OYUNCU ÖLDÜ! GAME OVER.");
        // Buraya Game Over ekraný kodu veya sahne yenileme gelecek
        // Time.timeScale = 0;
    }
}