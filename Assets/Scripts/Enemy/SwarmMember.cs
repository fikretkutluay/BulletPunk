using UnityEngine;

public class SwarmMember : MonoBehaviour 
{
    [Header("Movement")]
    public Vector3 moveDirection;
    public float speed = 7f;
    public float frequency = 5f; 
    public float magnitude = 2f; 
    
    [Header("Shooting")]
    public GameObject projectilePrefab;
    public float shootInterval = 1.5f;
    private float shootTimer;

    private float aliveTime;
    private Vector3 perpendicularDir;
    private Transform player;

    void Start() {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        // Calculate wiggle direction
        perpendicularDir = new Vector3(-moveDirection.y, moveDirection.x, 0);
        
        // Randomize shoot timer so they don't all fire at the exact same frame
        shootTimer = Random.Range(0f, shootInterval);
    }

    void Update() {
        aliveTime += Time.deltaTime;

        // 1. Snake Movement
        Vector3 forwardMove = moveDirection * speed * Time.deltaTime;
        Vector3 wiggleMove = perpendicularDir * Mathf.Sin(aliveTime * frequency) * magnitude * Time.deltaTime;
        transform.position += forwardMove + wiggleMove;

        // 2. Shooting Logic
        if (player != null) {
            shootTimer += Time.deltaTime;
            if (shootTimer >= shootInterval) {
                ShootAtPlayer();
                shootTimer = 0;
            }
        }

        // 3. Cleanup
        if (player != null && Vector2.Distance(transform.position, player.position) > 30f) {
            Destroy(gameObject);
        }
    }

    void ShootAtPlayer() {
        if (projectilePrefab == null || player == null) return;

        GameObject projGO = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        EnemyProjectile proj = projGO.GetComponent<EnemyProjectile>();

        // Set direction towards player
        Vector2 dir = (player.position - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        projGO.transform.rotation = Quaternion.Euler(0, 0, angle);

        // Since these are Ice Swarm, set the target key to Dash (Space)
        if (proj != null) {
            proj.targetKey = ControlType.Dash;
        }
    }
}