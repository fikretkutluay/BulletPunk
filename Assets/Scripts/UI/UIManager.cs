using UnityEngine;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [Header("Referanslar")]
    public PlayerSkillController playerSkillController; // Inspector'dan Player'ý sürükle
    public List<UIKeyDisplay> keyDisplays;

    void Update()
    {
        if (playerSkillController == null) return;

        foreach (var keyUI in keyDisplays)
        {
            // 1. Kilit Durumu (Herkes için geçerli)
            bool isActive = InputManager.Instance.IsControlActive(keyUI.controlType);
            bool isLocked = !isActive;

            // 2. Cooldown ve Seçilme (Sadece Skiller için geçerli)
            float cdRatio = 0f;
            bool isSelected = false;

            if (keyUI.isSkillKey)
            {
                cdRatio = playerSkillController.GetCooldownRatio(keyUI.controlType);
                isSelected = playerSkillController.IsSkillSelected(keyUI.controlType);
            }

            // 3. UI'ý Güncelle
            keyUI.UpdateDisplay(isLocked, cdRatio, isSelected);
        }
    }
}