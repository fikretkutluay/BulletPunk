using UnityEngine;
using System.Collections; // Coroutine için gerekli

public class PlayerSkillController : MonoBehaviour
{
    [Header("Skill Slotlarý")]
    [SerializeField] private SkillData skillQ;
    [SerializeField] private SkillData skillE;
    [SerializeField] private SkillData skillR;
    [SerializeField] private Animator animator;
    [Header("Niþangah Ayarlarý")]
    [SerializeField] private Transform reticleTransform; // Sahnedeki niþan görseli
    [SerializeField] private LayerMask groundLayer; // Raycast'in çarpacaðý zemin layer'ý

    // Þuan elimizde hangi skill var?
    private SkillData currentActiveSkill;
    private SkillData lastFiredSkill;
    private bool isAiming = false;

    // Cooldown takibi (Basit sözlük mantýðý)
    private float cooldownQ_Timer = 0;
    private float cooldownE_Timer = 0;
    private float cooldownR_Timer = 0;
    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();  
    }
    void Update()
    {
        HandleCooldowns();

        // Eðer niþan almýyorsak, tuþlarý dinle
        if (!isAiming)
        {
            if (Input.GetKeyDown(KeyCode.Q) && CheckSkillReady(skillQ, cooldownQ_Timer, ControlType.Skill1))
            {
                StartAiming(skillQ);
            }
            else if (Input.GetKeyDown(KeyCode.E) && CheckSkillReady(skillE, cooldownE_Timer, ControlType.Skill2))
            {
                StartAiming(skillE);
            }
            else if (Input.GetKeyDown(KeyCode.R) && CheckSkillReady(skillR, cooldownR_Timer, ControlType.Skill3))
            {
                StartAiming(skillR);
            }
        }
        else
        {
            // Niþan alma modundayýz
            UpdateReticlePosition();

            // Sol Týk: Ateþle
            if (Input.GetMouseButtonDown(0))
            {
                FireSkill();
            }
            // Sað Týk: Ýptal Et
            else if (Input.GetMouseButtonDown(1))
            {
                CancelAiming();
            }
        }
    }

    // Skill kullanýlabilir mi kontrolü
    private bool CheckSkillReady(SkillData data, float timer, ControlType type)
    {
        if (data == null) return false; // Skill atanmamýþ
        if (timer > 0) return false;    // Cooldown'da
        if (!InputManager.Instance.IsControlActive(type)) return false; // Debuff yemiþ!

        return true;
    }

    private void StartAiming(SkillData skill)
    {
        currentActiveSkill = skill;
        isAiming = true;

        if (reticleTransform != null)
            reticleTransform.gameObject.SetActive(true);

        Debug.Log(skill.skillName + " için niþan alýnýyor...");
    }

    private void UpdateReticlePosition()
    {
        // 1. Mouse'un dünyadaki yerini bul
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        if (reticleTransform != null)
        {
            // 2. Oyuncudan Mouse'a doðru olan vektörü hesapla
            Vector3 direction = mousePos - transform.position;

            // 3. Mesafeyi (Magnitude) al
            float distance = direction.magnitude;

            // 4. Mesafeyi Min ve Max deðerleri arasýna sýkýþtýr (Clamp)
            // Eðer mesafe minRange'den kýsaysa minRange olur, maxRange'den uzunsa maxRange olur.
            float clampedDistance = Mathf.Clamp(distance, currentActiveSkill.minRange, currentActiveSkill.maxRange);

            // 5. Yönü (normalized) koruyarak, yeni mesafe ile çarpýp pozisyonu ayarla
            // Eðer mouse tam karakterin üzerindeyse (direction 0 ise) varsayýlan olarak saða (veya baktýðý yöne) itelim
            Vector3 finalDirection = distance > 0.01f ? direction.normalized : Vector3.right;

            reticleTransform.position = transform.position + (finalDirection * clampedDistance);
        }
    }

    // 1. AÞAMA: Týklayýnca çalýþan kýsým
    private void FireSkill()
    {
        if (currentActiveSkill == null) return;
        lastFiredSkill = currentActiveSkill;

        // Oyuncuyu niþangaha döndür (Opsiyonel: Ateþ ederken fareye dönsün istiyorsan)
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (mousePos.x < transform.position.x) GetComponent<SpriteRenderer>().flipX = true;
        else GetComponent<SpriteRenderer>().flipX = false;

        // SADECE ANÝMASYONU TETÝKLE
        // SkillData'ya yazdýðýmýz ismi (örn: "AttackWave") kullanýyoruz.
        if (!string.IsNullOrEmpty(currentActiveSkill.animTriggerName))
        {
            animator.SetTrigger(currentActiveSkill.animTriggerName);
        }

        // NOT: Instantiate kodunu buradan sildik! Aþaðýdaki fonksiyona taþýdýk.

        // Cooldown'ý baþlat ve niþaný kapat
        if (currentActiveSkill == skillQ) cooldownQ_Timer = skillQ.cooldown;
        else if (currentActiveSkill == skillE) cooldownE_Timer = skillE.cooldown;
        else if (currentActiveSkill == skillR) cooldownR_Timer = skillR.cooldown;

        CancelAiming();
    }

    // 2. AÞAMA: Animasyonun içinden çaðrýlacak fonksiyon
    // Bu fonksiyonu Unity Editör'de Animation Event olarak seçeceðiz.
    public void SpawnProjectileFromAnim()
    {
        // Eðer skill iptal olduysa veya null ise yapma
        if (lastFiredSkill == null) return;

        Vector3 spawnPos = transform.position;

        // Küçük bir tüyo: Mouse o an neredeyse oraya atar. 
        // Eðer oyuncu animasyon sýrasýnda mouse'u çevirirse mermi yön deðiþtirir.
        // Ýstersen FireSkill'de yönü de yedekleyebilirsin ama þimdilik böyle kalsýn.
        Vector3 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetPos.z = 0;

        Vector2 direction = (targetPos - spawnPos).normalized;

        // DÜZELTME: Artýk 'lastFiredSkill' kullanýyoruz
        if (lastFiredSkill.projectilePrefab != null)
        {
            GameObject projectileObj = Instantiate(lastFiredSkill.projectilePrefab, spawnPos, Quaternion.identity);
            Projectile projectileScript = projectileObj.GetComponent<Projectile>();

            if (projectileScript != null)
            {
                // Hasar bilgisini de 'lastFiredSkill'den çekiyoruz
                projectileScript.Initialize(direction, 15f, lastFiredSkill.damage);
            }
        }
    }
    private void CancelAiming()
    {
        isAiming = false;
        currentActiveSkill = null;
        if (reticleTransform != null)
            reticleTransform.gameObject.SetActive(false);
    }

    private void HandleCooldowns()
    {
        if (cooldownQ_Timer > 0) cooldownQ_Timer -= Time.deltaTime;
        if (cooldownE_Timer > 0) cooldownE_Timer -= Time.deltaTime;
        if (cooldownR_Timer > 0) cooldownR_Timer -= Time.deltaTime;
    }
}