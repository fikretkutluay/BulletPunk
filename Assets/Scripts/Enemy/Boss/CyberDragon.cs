using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class CyberDragon : MonoBehaviour, IDamageable
{
    [Header("Dragon Structure")]
    public GameObject segmentPrefab;
    public int segmentCount = 15;
    public float segmentDistance = 0.4f;
    public float moveSpeed = 5f;

    [Header("Stats & UI")]
    public float health = 500f;
    public GameObject healthBarPrefab;
    private Slider healthSlider;

    [Header("Combat")]
    public GameObject projectilePrefab;
    public float shootInterval = 2f;
    public int shootEveryXSegments = 4;

    [Header("Visual Effects")]
    public float flashDuration = 0.1f;
    private List<Material> allMaterials = new List<Material>();

    [Header("Movement")]
    public float swingSpeed = 5f;
    public float swingAmount = 0.5f;
    public float waveOffset = 0.2f;
    public float pointRadius = 7f;
    public float arrivalDistance = 1.5f;

    private List<Transform> bodyParts = new List<Transform>();
    private Transform player;
    private Vector3 currentTargetPoint;
    private float timeCounter;
    private float shootTimer;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        bodyParts.Add(this.transform);

        // 1. Spawn Body
        for (int i = 0; i < segmentCount; i++)
        {
            GameObject seg = Instantiate(segmentPrefab, transform.position, Quaternion.identity);
            bodyParts.Add(seg.transform);

            float scaleMultiplier = 1.0f - ((float)i / segmentCount) * 0.6f;
            seg.transform.localScale = transform.localScale * scaleMultiplier;
        }

        // 2. Setup Health Bar
        if (healthBarPrefab != null)
        {
            GameObject hb = Instantiate(healthBarPrefab, transform.position + Vector3.up * 1f, Quaternion.identity, transform);
            healthSlider = hb.GetComponentInChildren<Slider>();
            if (healthSlider) { healthSlider.maxValue = health; healthSlider.value = health; }
        }

        // 3. Collect ALL materials (Head + Segments) for flashing
        foreach (Transform t in bodyParts)
        {
            SpriteRenderer sr = t.GetComponent<SpriteRenderer>();
            if (sr != null) allMaterials.Add(sr.material);
        }

        SetNewTargetPoint();
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
    }

    // --- DAMAGE LOGIC ---
    public void TakeDamage(float amount)
    {
        health -= amount;
        if (healthSlider) healthSlider.value = health;

        StopCoroutine("DamageFlashRoutine");
        StartCoroutine(DamageFlashRoutine());

        if (health <= 0)
        {
            Die(); // Calling the missing method
        }
    }

    private IEnumerator DamageFlashRoutine()
    {
        foreach (Material mat in allMaterials) mat.SetFloat("_FlashAmount", 0.259f);
        yield return new WaitForSeconds(flashDuration);
        foreach (Material mat in allMaterials) mat.SetFloat("_FlashAmount", 0f);
    }

    // --- THE DIE METHOD ---
    private void Die()
    {
        // 1. Clean up all segments
        foreach (Transform t in bodyParts)
        {
            if (t != null && t != transform)
            {
                // Optional: Spawn a small hit effect at each segment position
                Destroy(t.gameObject);
            }
        }

        // 2. (Optional) Spawn a big explosion or lots of XP at the head position
        Debug.Log("Boss Defeated!");

        // 3. Destroy the head itself
        Destroy(gameObject);
    }

    // --- MOVEMENT & COMBAT (Logic remains the same) ---
    void FireFromSegments()
    {
        for (int i = 1; i < bodyParts.Count; i++)
        {
            if (i % shootEveryXSegments == 0) SpawnBullet(bodyParts[i]);
        }
    }

    void SpawnBullet(Transform source)
    {
        if (projectilePrefab == null) return;
        GameObject projGO = Instantiate(projectilePrefab, source.position, Quaternion.identity);
        EnemyProjectile proj = projGO.GetComponent<EnemyProjectile>();
        Vector2 dir = (player.position - source.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        projGO.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void MoveHead()
    {
        float distToPoint = Vector3.Distance(transform.position, currentTargetPoint);
        if (distToPoint < arrivalDistance) SetNewTargetPoint();
        Vector3 dir = (currentTargetPoint - transform.position).normalized;
        Vector3 sideDir = new Vector3(-dir.y, dir.x, 0);
        Vector3 wiggle = sideDir * Mathf.Sin(timeCounter * swingSpeed) * swingAmount;
        transform.position += (dir * moveSpeed + wiggle) * Time.deltaTime;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle), Time.deltaTime * 5f);
    }

    void SetNewTargetPoint()
    {
        // 1. Pick a random direction (Vector2 with length of 1)
        // .normalized ensures the point is ON the circle, not INSIDE it
        Vector2 randomDirection = Random.insideUnitCircle.normalized;

        // 2. Multiply by the radius to get the exact border point
        Vector3 offset = (Vector3)(randomDirection * pointRadius);

        // 3. Set the target relative to the player
        currentTargetPoint = player.position + offset;

        // Optional: Log for debug
        // Debug.Log("New Border Target Set: " + currentTargetPoint);
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
                float wave = Mathf.Sin((timeCounter * swingSpeed) - (i * waveOffset));
                float tailMultiplier = (float)i / segmentCount;
                current.position = targetPos + sideDir * wave * swingAmount * tailMultiplier;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
                current.rotation = Quaternion.Slerp(current.rotation, Quaternion.Euler(0, 0, angle), Time.deltaTime * 10f);
            }
        }
    }
}