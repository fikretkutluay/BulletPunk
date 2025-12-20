using UnityEngine;

// Bu script oyunda bir obje deðil, Asset klasöründe dosya olacak.
[CreateAssetMenu(fileName = "New Skill", menuName = "Game/Skill Data")]
public class SkillData : ScriptableObject
{
    [Header("Genel Bilgiler")]
    public string skillName;
    public Sprite icon; // UI için
    public float cooldown = 2f;

    [Header("Oynanýþ")]
    public GameObject projectilePrefab; // Atýlacak mermi/efekt
    public float damage = 10f;

    [Header("Menzil Ayarlarý")]
    public float minRange = 1f;
    public float maxRange = 10f; // Ne kadar uzaða atýlabilir?

    [Header("Animasyon")]
    public string animTriggerName;
}