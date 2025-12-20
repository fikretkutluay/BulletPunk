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
    // groundLayer sildim çünkü kullanmýyorsun.

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
    private Vector3 savedTargetPosition;

    // Component Referanslarý (Performans için)
    private SpriteRenderer spriteRenderer;

    // Cooldown Sayaçlarý
    private float cooldownQ_Timer = 0;
    private float cooldownE_Timer = 0;
    private float cooldownR_Timer = 0;

    private void Awake()
    {
        // Componentleri bir kere bulup hafýzaya atýyoruz (Caching)
        if (animator == null) animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        HandleCooldowns();

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
            UpdateReticlePosition();

            if (Input.GetMouseButtonDown(0))
            {
                FireSkill();
            }
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
        if (currentActiveSkill == skillR) return;

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

    // --- 1. AÞAMA: ATEÞLEME ---
    private void FireSkill()
    {
        if (currentActiveSkill == null) return;
        lastFiredSkill = currentActiveSkill;

        // Hedef kaydetme
        if (reticleTransform != null && reticleTransform.gameObject.activeSelf)
            savedTargetPosition = reticleTransform.position;
        else
            savedTargetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Karakteri Döndür (Artýk cache'lenen spriteRenderer'ý kullanýyoruz)
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (mouseWorldPos.x < transform.position.x)
            spriteRenderer.flipX = true;
        else
            spriteRenderer.flipX = false;

        // Animasyon
        if (!string.IsNullOrEmpty(currentActiveSkill.animTriggerName))
        {
            animator.SetTrigger(currentActiveSkill.animTriggerName);
        }

        // Cooldown
        if (currentActiveSkill == skillQ) cooldownQ_Timer = skillQ.cooldown;
        else if (currentActiveSkill == skillE) cooldownE_Timer = skillE.cooldown;
        else if (currentActiveSkill == skillR) cooldownR_Timer = skillR.cooldown;

        CancelAiming();
    }

    // --- 2. AÞAMA: ANIMATION EVENT ---
    public void SpawnProjectileFromAnim()
    {
        if (lastFiredSkill == null) return;

        // >>> SKILL R (ULTIMATE) <<<
        if (lastFiredSkill == skillR)
        {
            // Cache'lenen renderer'ý kullanýyoruz
            Vector2 facingDir = spriteRenderer.flipX ? Vector2.left : Vector2.right;

            Collider2D[] allHits = Physics2D.OverlapCircleAll(transform.position, ultiRadius);

            foreach (Collider2D hit in allHits)
            {
                if (hit.gameObject == gameObject) continue;

                Vector2 dirToTarget = (hit.transform.position - transform.position).normalized;

                if (Vector2.Angle(facingDir, dirToTarget) < ultiAngle / 2f)
                {
                    IDamageable damageable = hit.GetComponentInParent<IDamageable>();
                    if (damageable != null)
                    {
                        damageable.TakeDamage(lastFiredSkill.damage);
                    }
                }
            }
        }
        // >>> SKILL E (YILDIRIM) <<<
        else if (lastFiredSkill == skillE)
        {
            if (lastFiredSkill.projectilePrefab != null)
            {
                GameObject lightningObj = Instantiate(lastFiredSkill.projectilePrefab, savedTargetPosition, Quaternion.identity);
                LightningSpell spellScript = lightningObj.GetComponent<LightningSpell>();
                if (spellScript != null)
                {
                    spellScript.Initialize(lastFiredSkill.damage, 3.0f);
                }
            }
        }
        // >>> SKILL Q (MERMI) <<<
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

    // Cooldown
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

    // Gizmos
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f);

        // Editor modunda Awake çalýþmadýðý için burada GetComponent mecburidir
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Vector3 facingDir = (sr != null && sr.flipX) ? Vector3.left : Vector3.right;

        Quaternion leftRot = Quaternion.AngleAxis(-ultiAngle / 2f, Vector3.forward);
        Quaternion rightRot = Quaternion.AngleAxis(ultiAngle / 2f, Vector3.forward);
        Gizmos.DrawRay(transform.position, (leftRot * facingDir) * ultiRadius);
        Gizmos.DrawRay(transform.position, (rightRot * facingDir) * ultiRadius);

        Gizmos.DrawWireSphere(transform.position, ultiRadius);
    }
}