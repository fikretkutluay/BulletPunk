using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Hareket Ayarlarý")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Dash (Atýlma) Ayarlarý")]
    [SerializeField] private float dashSpeed = 20f;       // Atýlma hýzý
    [SerializeField] private float dashDuration = 0.2f;   // Ne kadar sürecek?
    [SerializeField] private float dashCooldown = 1f;     // Ne sýklýkla atýlabilir?
    [SerializeField] private bool invincibleDuringDash = true; // Dash atarken hasar almazlýk olsun mu?

    [Header("Efektler ve Katmanlar")]
    [SerializeField] private TrailRenderer trailRenderer; // Inspector'dan atanacak
    [SerializeField] private string enemyLayerName = "Enemy"; // Çarpýþmayý kapatacaðýmýz katman adý

    // Dash Durum Deðiþkenleri
    private bool isDashing = false;
    private float dashTimer = 0f;
    private int playerLayer;
    private int enemyLayer;

    [Header("Görsel Ayarlar")]
    [SerializeField] private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Vector2 movementInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (animator == null) animator = GetComponent<Animator>();
        if (trailRenderer == null) trailRenderer = GetComponent<TrailRenderer>(); // Eðer ekli ama atanmamýþsa bulmaya çalýþ

        // Katman numaralarýný alýyoruz
        playerLayer = gameObject.layer;
        enemyLayer = LayerMask.NameToLayer(enemyLayerName);
    }

    void Update()
    {
        if (dashTimer > 0)
        {
            dashTimer -= Time.deltaTime;
        }

        // --- DASH GÝRÝÞÝ ---
        // Space'e basýldý mý? + Cooldown bitti mi? + InputManager izin veriyor mu?
        if (Input.GetKeyDown(KeyCode.Space) && dashTimer <= 0 && InputManager.Instance.IsControlActive(ControlType.Dash))
        {
            StartCoroutine(PerformDash());
        }

        // Eðer Dash atýyorsak, hareket kodlarýný çalýþtýrma
        if (isDashing) return;

        // Normal Hareketler
        float moveX = 0f;
        float moveY = 0f;

        // --- YUKARI (W) ---
        if (Input.GetKey(KeyCode.W) && InputManager.Instance.IsControlActive(ControlType.MoveUp))
            moveY = 1f;

        // --- AÞAÐI (S) ---
        if (Input.GetKey(KeyCode.S) && InputManager.Instance.IsControlActive(ControlType.MoveDown))
            moveY = -1f;

        // --- SOL (A) ---
        if (Input.GetKey(KeyCode.A) && InputManager.Instance.IsControlActive(ControlType.MoveLeft))
            moveX = -1f;

        // --- SAÐ (D) ---
        if (Input.GetKey(KeyCode.D) && InputManager.Instance.IsControlActive(ControlType.MoveRight))
            moveX = 1f;

        movementInput = new Vector2(moveX, moveY).normalized;

        if (moveX != 0)
        {
            spriteRenderer.flipX = (moveX < 0);
        }

        // --- ANIMASYON GÜNCELLEME ---
        if (animator != null)
        {
            // Koþma animasyonu için
            animator.SetFloat("moveX", movementInput.magnitude);
        }
    }

    void FixedUpdate()
    {
        if (isDashing) return;

        rb.linearVelocity = movementInput * moveSpeed;
    }

    // --- DASH FONKSÝYONU ---
    private IEnumerator PerformDash()
    {
        isDashing = true;
        dashTimer = dashCooldown;

        // 1. Dash Baþlangýcý: Efektleri Aç
        if (trailRenderer != null) trailRenderer.emitting = true;

        // 2. Dash Baþlangýcý: Animasyonu Tetikle (Önceki konuþmamýzdaki trigger)
        if (animator != null) animator.SetTrigger("Dash");

        // 3. Dash Baþlangýcý: Çarpýþmayý Kapat (Düþmanlarýn içinden geç)
        if (enemyLayer != -1)
            Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);

        // Hangi yöne dash atacaðýz?
        Vector2 dashDirection = movementInput;

        // Eðer duruyorsak mouse yönüne atýl
        if (dashDirection == Vector2.zero)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            dashDirection = (mousePos - transform.position).normalized;
        }

        // Dash Hýzýný Uygula
        rb.linearVelocity = dashDirection * dashSpeed;

        // Süre kadar bekle
        yield return new WaitForSeconds(dashDuration);

        // --- DASH BÝTÝÞÝ ---
        isDashing = false;
        rb.linearVelocity = Vector2.zero;

        // 4. Bitiþ: Efektleri Kapat
        if (trailRenderer != null) trailRenderer.emitting = false;

        // 5. Bitiþ: Çarpýþmayý Geri Aç
        if (enemyLayer != -1)
            Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);
    }
}