using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class LevelUpManager : MonoBehaviour
{
    public static LevelUpManager Instance;

    [Header("UI Panels")]
    public GameObject levelUpPanel;
    public Button[] upgradeButtons;
    public TextMeshProUGUI[] buttonTexts;

    [Header("Upgrade List")]
    public List<UpgradeOption> availableUpgrades;

    private void Awake() => Instance = this;

    public void ShowLevelUpMenu()
    {
        levelUpPanel.SetActive(true);
        Time.timeScale = 0f; // Stop the game

        List<UpgradeOption> selectedOptions = GetRandomUpgrades(3);

        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            int index = i; // Closure fix
            buttonTexts[i].text = $"{selectedOptions[i].upgradeName}\n<size=20>{selectedOptions[i].description}</size>";

            upgradeButtons[i].onClick.RemoveAllListeners();
            upgradeButtons[i].onClick.AddListener(() => ApplyUpgrade(selectedOptions[index]));
        }
    }

    private List<UpgradeOption> GetRandomUpgrades(int count)
    {
        List<UpgradeOption> shuffled = new List<UpgradeOption>(availableUpgrades);
        for (int i = 0; i < shuffled.Count; i++)
        {
            UpgradeOption temp = shuffled[i];
            int randomIndex = Random.Range(i, shuffled.Count);
            shuffled[i] = shuffled[randomIndex];
            shuffled[randomIndex] = temp;
        }
        return shuffled.GetRange(0, Mathf.Min(count, shuffled.Count));
    }

    public void ApplyUpgrade(UpgradeOption option)
    {
        PlayerStats player = FindFirstObjectByType<PlayerStats>();

        switch (option.type)
        {
            case UpgradeType.Health_Max:
                player.maxHealth += 20;
                player.currentHealth = player.maxHealth; // Full heal
                break;
            case UpgradeType.Q_Damage:
                player.qDamageMultiplier += 0.2f; // +20% Damage
                break;
            case UpgradeType.E_Damage:
                player.eDamageMultiplier += 0.2f;
                break;
            case UpgradeType.R_Damage:
                player.rDamageMultiplier += 0.2f;
                break;
        }

        levelUpPanel.SetActive(false);
        Time.timeScale = 1f; // Resume game
    }
}