using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class CyberDragon : MonoBehaviour, IDamageable
{
    [Header("Dragon Structure")]
    public GameObject segmentPrefab;
    public Sprite tailSprite;
    public int segmentCount = 15;
    public float segmentDistance = 0.4f;
    public float moveSpeed = 5f;

    [Header("Stats & UI")]
    public float health = 500f;
    public GameObject healthBarPrefab;
    private Slider healthSlider;
    [SerializeField] private float healthBarOffsetY = 1.2f;

    [Header("Combat")]
    public GameObject projectilePrefab;
    public float shootInterval = 2f;
    public int shootEveryXSegments = 4;

    [Header("Visual Effects (Shader)")]
    public float flashDuration = 0.1f;
    private List<Material> allMaterials = new List<Material>();

    [Header("Tail Swing & Pathfinding")]
    public float swingSpeed = 5f;
    public float swingAmount = 0.5f;
    public float waveOffset = 0.2f;
    public float pointRadius = 8f;
    public float arrivalDistance = 1.5f;

    private List<Transform> bodyParts = new List<Transform>();
    private Transform player;
    private Vector3 currentTargetPoint;
    private float timeCounter;
    private float shootTimer;

    private void Awake()
    {
        // We handle material collection in Start after segments exist
    }

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        InitializeDragon();
        SetupHealthBar();
        CollectMaterials();
        SetNewTargetPoint();
    }

    void InitializeDragon()
    {
        bodyParts.Clear();
        bodyParts.Add(this.transform);

        for (int i = 0; i < segmentCount; i++)
        {
            GameObject seg = Instantiate(segmentPrefab, transform.position, Quaternion.identity);
            seg.name = "Segment_" + i;
            bodyParts.Add(seg.transform);

            // Handle Tail Sprite
            if (i == segmentCount - 1 && tailSprite != null)
            {
                SpriteRenderer sr = seg.GetComponent<SpriteRenderer>();
                if (sr != null) sr.sprite = tailSprite;
            }

            // Taper scale toward tail
            float scaleMultiplier = 1.0f - ((float)i / segmentCount) * 0.6f;
            seg.transform.localScale = transform.localScale * scaleMultiplier;

            // Set sorting order so segments stay behind head
            SpriteRenderer segmentSR = seg.GetComponent<SpriteRenderer>();
            if (segmentSR != null) segmentSR.sortingOrder = -i;
        }
    }

    void SetupHealthBar()
    {
        if (healthBarPrefab != null)
        {
            GameObject hb = Instantiate(healthBarPrefab, transform.position + Vector3.up * healthBarOffsetY, Quaternion.identity, transform);
            healthSlider = hb.GetComponentInChildren<Slider>();
            if (healthSlider)
            {
                healthSlider.maxValue = health;
                healthSlider.value = health;
            }
        }
    }

    void CollectMaterials()
    {
        foreach (Transform t in bodyParts)
        {
            SpriteRenderer sr = t.GetComponent<SpriteRenderer>();
            if (sr != null) allMaterials.Add(sr.material);
        }
    }

    void Update()
    {
        if (player == null) return;
        timeCounter += Time.deltaTime;
        shootTimer += Time.deltaTime;

        MoveHead();
        MoveBody();

        if (shootTimer >= shootInterval)
        {
            FireFromSegments();
            shootTimer = 0;
        }

        // Keep Health Bar from rotating with the head
        if (healthSlider != null) healthSlider.transform.parent.rotation = Quaternion.identity;
    }

    void MoveHead()
    {
        float distToPoint = Vector3.Distance(transform.position, currentTargetPoint);
        if (distToPoint < arrivalDistance) SetNewTargetPoint();

        Vector3 dir = (currentTargetPoint - transform.position).normalized;
        Vector3 sideDir = new Vector3(-dir.y, dir.x, 0);
        Vector3 wiggle = sideDir * Mathf.Sin(timeCounter * swingSpeed) * swingAmount;

        transform.position += (dir * moveSpeed + wiggle) * Time.deltaTime;

        // --- UPDATED ROTATION LOGIC ---
        if (player != null)
        {
            // 1. Calculate direction specifically toward the PLAYER
            Vector3 dirToPlayer = (player.position - transform.position).normalized;

            // 2. Calculate the angle (adding -90 if your sprite faces 'Up' by default)
            float angle = Mathf.Atan2(dirToPlayer.y, dirToPlayer.x) * Mathf.Rad2Deg - 90f;

            // 3. Apply the rotation (Slerp makes the turn smooth)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle), Time.deltaTime * 10f);
        }
    }

    void SetNewTargetPoint()
    {
        if (player == null) return;

        // Force target to the opposite half of the circle
        Vector2 dirFromPlayer = (transform.position - player.position).normalized;
        Vector2 oppositeDir = -dirFromPlayer;

        // Add 45 degree random variation
        float randomAngle = Random.Range(-45f, 45f);
        Quaternion spreadRotation = Quaternion.Euler(0, 0, randomAngle);
        Vector2 finalDir = spreadRotation * oppositeDir;

        currentTargetPoint = player.position + (Vector3)(finalDir.normalized * pointRadius);
    }

    void MoveBody()
    {
        for (int i = 1; i < bodyParts.Count; i++)
        {
            Transform current = bodyParts[i];
            Transform target = bodyParts[i - 1];

            float distance = Vector3.Distance(current.position, target.position);
            Vector3 direction = (target.position - current.position).normalized;

            if (distance > segmentDistance)
            {
                Vector3 targetPos = target.position - (direction * segmentDistance);
                Vector3 sideDir = new Vector3(-direction.y, direction.x, 0);

                // Sine wave offset for the "S" curve swing
                float wave = Mathf.Sin((timeCounter * swingSpeed) - (i * waveOffset));
                float tailMultiplier = (float)i / segmentCount;

                current.position = targetPos + (sideDir * wave * swingAmount * tailMultiplier);

                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
                current.rotation = Quaternion.Slerp(current.rotation, Quaternion.Euler(0, 0, angle), Time.deltaTime * 10f);
            }
        }
    }



    void FireFromSegments()
    {
        for (int i = 1; i < bodyParts.Count; i++)
        {
            if (i % shootEveryXSegments == 0)
            {
                SpawnBullet(bodyParts[i]);
            }
        }
    }

    void SpawnBullet(Transform source)
    {
        if (projectilePrefab == null) return;
        GameObject projGO = Instantiate(projectilePrefab, source.position, Quaternion.identity);
        EnemyProjectile proj = projGO.GetComponent<EnemyProjectile>();

        if (proj != null)
        {
            Vector2 dir = (player.position - source.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            projGO.transform.rotation = Quaternion.Euler(0, 0, angle);
            proj.targetKey = ControlType.Skill2;
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (healthSlider) healthSlider.value = health;

        StopCoroutine("DamageFlashRoutine");
        StartCoroutine(DamageFlashRoutine());

        if (health <= 0) Die();
    }

    private IEnumerator DamageFlashRoutine()
    {
        foreach (Material mat in allMaterials) mat.SetFloat("_FlashAmount", 0.259f);
        yield return new WaitForSeconds(flashDuration);
        foreach (Material mat in allMaterials) mat.SetFloat("_FlashAmount", 0f);
    }

    private void Die()
    {
        foreach (Transform t in bodyParts)
        {
            if (t != null && t != transform) Destroy(t.gameObject);
        }
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        if (player != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(player.position, pointRadius);
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(currentTargetPoint, 0.4f);
        }
    }
}