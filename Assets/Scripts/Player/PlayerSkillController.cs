using UnityEngine;
using System.Collections;

public class PlayerSkillController : MonoBehaviour
{
    [Header("Skill Slotlarý")]
    [SerializeField] private SkillData skillQ; // Mermi
    [SerializeField] private SkillData skillE; // Yýldýrým (Alan)
    [SerializeField] private SkillData skillR; // Ultimate (Yarým Daire Yok Etme)
    [SerializeField] private Animator animator;

    [Header("Niþangah Ayarlarý")]
    [SerializeField] private Transform reticleTransform;
    [SerializeField] private LayerMask groundLayer;

    [Header("Ultimate (R) Ayarlarý")]
    [Tooltip("Ultinin ne kadar uzaða vuracaðý")]
    public float ultiRadius = 7f;
    [Tooltip("Ultinin açýsý (180 yaparsan tam yarým daire olur)")]
    [Range(0, 360)]
    public float ultiAngle = 180f;
    [Tooltip("Ultinin kimlere vuracaðýný seç (Enemy seçmelisin)")]
    public LayerMask enemyLayer;

    // Durum Deðiþkenleri
    private SkillData currentActiveSkill;
    private SkillData lastFiredSkill;
    private bool isAiming = false;
    private Vector3 savedTargetPosition; // E skilli için hafýza

    // Cooldown Sayaçlarý
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

        if (!isAiming)
        {
            // Skill Seçimi
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
                // R için niþan almaya gerek yok, direkt çalýþtýrabiliriz ama
                // senin yapýnda niþan moduyla tetiklendiði için buraya alýyorum.
                StartAiming(skillR);
            }
        }
        else
        {
            // Niþangah varsa güncelle (R için niþangah gizlenebilir, aþaðýda ayarladýk)
            UpdateReticlePosition();

            // Sol Týk: Ateþle
            if (Input.GetMouseButtonDown(0))
            {
                FireSkill();
            }
            // Sað Týk: Ýptal
            else if (Input.GetMouseButtonDown(1))
            {
                CancelAiming();
            }
        }
    }

    // --- YARDIMCI FONKSÝYONLAR ---
    private bool CheckSkillReady(SkillData data, float timer, ControlType type)
    {
        if (data == null) return false;
        if (timer > 0) return false;
        if (!InputManager.Instance.IsControlActive(type)) return false;
        return true;
    }

    private void StartAiming(SkillData skill)
    {
        currentActiveSkill = skill;
        isAiming = true;

        // EÐER SKILL R ÝSE NÝÞANGAHI GÖSTERME (Ýsteðe baðlý)
        // Çünkü karakterin önüne vuracak, mouse'a deðil.
        if (skill == skillR)
        {
            if (reticleTransform != null) reticleTransform.gameObject.SetActive(false);
        }
        else
        {
            if (reticleTransform != null) reticleTransform.gameObject.SetActive(true);
        }
    }

    private void UpdateReticlePosition()
    {
        if (currentActiveSkill == skillR) return; // R skilli mouse takip etmez

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        if (reticleTransform != null)
        {
            Vector3 direction = mousePos - transform.position;
            float distance = direction.magnitude;
            float clampedDistance = Mathf.Clamp(distance, currentActiveSkill.minRange, currentActiveSkill.maxRange);
            Vector3 finalDirection = distance > 0.01f ? direction.normalized : Vector3.right;

            reticleTransform.position = transform.position + (finalDirection * clampedDistance);
        }
    }

    // --- 1. AÞAMA: ATEÞLEME (Animasyon Tetikleme) ---
    private void FireSkill()
    {
        if (currentActiveSkill == null) return;
        lastFiredSkill = currentActiveSkill;

        // Hedef pozisyonu kaydet (E skilli için lazým)
        if (reticleTransform != null && reticleTransform.gameObject.activeSelf)
            savedTargetPosition = reticleTransform.position;
        else
            savedTargetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); // R için mouse yönü alýnabilir veya karakter yönü

        // Karakterin yönünü çevir (R skilli için mouse tarafýna dönmesi iyi olur)
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (mouseWorldPos.x < transform.position.x)
            GetComponent<SpriteRenderer>().flipX = true;
        else
            GetComponent<SpriteRenderer>().flipX = false;

        // Animasyonu Baþlat
        if (!string.IsNullOrEmpty(currentActiveSkill.animTriggerName))
        {
            animator.SetTrigger(currentActiveSkill.animTriggerName);
        }

        // Cooldown Baþlat
        if (currentActiveSkill == skillQ) cooldownQ_Timer = skillQ.cooldown;
        else if (currentActiveSkill == skillE) cooldownE_Timer = skillE.cooldown;
        else if (currentActiveSkill == skillR) cooldownR_Timer = skillR.cooldown;

        CancelAiming();
    }

    // --- 2. AÞAMA: ANÝMASYON EVENT ÝLE ÇAÐRILAN HASAR KODU ---
    // Animation Event burayý tetikleyecek
    public void SpawnProjectileFromAnim()
    {
        if (lastFiredSkill == null) return;

        // >>> SKILL R (ULTIMATE - HER ÞEYÝ YOK ET) <<<
        if (lastFiredSkill == skillR)
        {
            Vector2 facingDir = GetComponent<SpriteRenderer>().flipX ? Vector2.left : Vector2.right;

            // 1. Layer filtresi YOK. Yarýçaptaki HER Collider'ý alýyoruz.
            Collider2D[] allHits = Physics2D.OverlapCircleAll(transform.position, ultiRadius);

            foreach (Collider2D hit in allHits)
            {
                // Kendimize vurmayalým
                if (hit.gameObject == gameObject) continue;

                // 2. Açý Kontrolü (Koni içinde mi?)
                Vector2 dirToTarget = (hit.transform.position - transform.position).normalized;

                if (Vector2.Angle(facingDir, dirToTarget) < ultiAngle / 2f)
                {
                    // 3. IDamageable ara (Hem objenin kendisine hem babasýna bak)
                    // Bu sayede collider child'da olsa bile ana karakteri bulur.
                    IDamageable damageable = hit.GetComponentInParent<IDamageable>();

                    if (damageable != null)
                    {
                        // Bulduðun an yapýþtýr hasarý
                        damageable.TakeDamage(lastFiredSkill.damage);
                    }
                }
            }
        }

        // >>> SKILL E (YILDIRIM - ALAN ETKÝLÝ) <<<
        else if (lastFiredSkill == skillE)
        {
            if (lastFiredSkill.projectilePrefab != null)
            {
                GameObject lightningObj = Instantiate(lastFiredSkill.projectilePrefab, savedTargetPosition, Quaternion.identity);
                // LightningSpell scriptini arýyoruz
                LightningSpell spellScript = lightningObj.GetComponent<LightningSpell>();
                if (spellScript != null)
                {
                    spellScript.Initialize(lastFiredSkill.damage, 3.0f);
                }
            }
        }

        // >>> SKILL Q (NORMAL MERMÝ) <<<
        else
        {
            Vector3 spawnPos = transform.position;
            Vector2 direction = (savedTargetPosition - spawnPos).normalized;

            if (lastFiredSkill.projectilePrefab != null)
            {
                GameObject projectileObj = Instantiate(lastFiredSkill.projectilePrefab, spawnPos, Quaternion.identity);
                Projectile projectileScript = projectileObj.GetComponent<Projectile>();
                if (projectileScript != null)
                {
                    projectileScript.Initialize(direction, 15f, lastFiredSkill.damage);
                }
            }
        }
    }

    // Cooldown yönetimi
    private void HandleCooldowns()
    {
        if (cooldownQ_Timer > 0) cooldownQ_Timer -= Time.deltaTime;
        if (cooldownE_Timer > 0) cooldownE_Timer -= Time.deltaTime;
        if (cooldownR_Timer > 0) cooldownR_Timer -= Time.deltaTime;
    }

    private void CancelAiming()
    {
        isAiming = false;
        currentActiveSkill = null;
        if (reticleTransform != null) reticleTransform.gameObject.SetActive(false);
    }

    // Editörde Ulti alanýný (Koniyi) görmek için
    private void OnDrawGizmosSelected()
    {
        // Ulti Alaný (Kýrmýzý)
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Vector3 facingDir = GetComponent<SpriteRenderer>().flipX ? Vector3.left : Vector3.right;

        // Koninin kenar çizgileri
        Quaternion leftRot = Quaternion.AngleAxis(-ultiAngle / 2f, Vector3.forward);
        Quaternion rightRot = Quaternion.AngleAxis(ultiAngle / 2f, Vector3.forward);
        Gizmos.DrawRay(transform.position, (leftRot * facingDir) * ultiRadius);
        Gizmos.DrawRay(transform.position, (rightRot * facingDir) * ultiRadius);

        // Menzil çemberi
        Gizmos.DrawWireSphere(transform.position, ultiRadius);
    }
}