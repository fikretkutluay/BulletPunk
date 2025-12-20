using UnityEngine;
using UnityEngine.UI;

public class UIKeyDisplay : MonoBehaviour
{
    [Header("Ayarlar")]
    public ControlType controlType;
    public bool isSkillKey = false;

    [Header("Görsel Referanslar")]
    public Image keyIcon;
    public GameObject lockIcon;
    public Image cooldownOverlay;
    public GameObject selectionGlow;

    [Header("Renkler")]
    public Color normalColor = Color.white;
    public Color lockedColor = Color.red;

    public void UpdateDisplay(bool isLocked, float cooldownRatio, bool isSelected)
    {
        // GÜVENLÝK KONTROLÜ: Eðer ana resim yoksa iþlem yapma!
        if (keyIcon == null) return;

        // 1. KÝLÝTLENME DURUMU
        if (isLocked)
        {
            keyIcon.color = lockedColor;
            if (lockIcon != null) lockIcon.SetActive(true);
            if (cooldownOverlay != null) cooldownOverlay.fillAmount = 0;
            if (selectionGlow != null) selectionGlow.SetActive(false);
            return;
        }

        // Kilit yoksa normale dön
        keyIcon.color = normalColor;
        if (lockIcon != null) lockIcon.SetActive(false);

        // 2. HAREKET TUÞLARI ÝÇÝN ÇIKIÞ
        if (!isSkillKey) return;

        // 3. SKILL TUÞLARI (Q E R)
        if (cooldownOverlay != null)
        {
            cooldownOverlay.fillAmount = cooldownRatio;
        }

        if (selectionGlow != null)
        {
            selectionGlow.SetActive(isSelected);
        }
    }
}