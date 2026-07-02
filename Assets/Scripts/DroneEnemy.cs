using UnityEngine;

/// <summary>
/// Drone que patrulla de izquierda a derecha y dispara proyectiles hacia abajo.
/// Ponélo en un GameObject en la escena y asigná el prefab del proyectil.
/// </summary>
public class DroneEnemy : MonoBehaviour
{
    [Header("Patrulla")]
    [Tooltip("Cuántas unidades se mueve hacia cada lado desde su posición inicial")]
    public float patrolDistance = 4f;
    public float moveSpeed = 3f;

    [Header("Disparo")]
    public GameObject projectilePrefab;
    [Tooltip("Segundos entre cada disparo")]
    public float fireInterval = 2f;
    [Tooltip("Punto desde donde sale el proyectil (opcional, usa el centro del drone si está vacío)")]
    public Transform firePoint;

    Vector3 startPos;
    float   direction = 1f;   // 1 = derecha, -1 = izquierda
    float   fireTimer;

    void Start()
    {
        startPos  = transform.position;
        fireTimer = fireInterval;
    }

    void Update()
    {
        Patrol();

        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0f)
        {
            Shoot();
            fireTimer = fireInterval;
        }
    }

    void Patrol()
    {
        transform.Translate(Vector2.right * direction * moveSpeed * Time.deltaTime);

        float distFromStart = transform.position.x - startPos.x;
        if (distFromStart >= patrolDistance)
            direction = -1f;
        else if (distFromStart <= -patrolDistance)
            direction = 1f;
    }

    void Shoot()
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning($"[DroneEnemy] {gameObject.name}: projectilePrefab es null, asignalo en el Inspector.");
            return;
        }

        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
        Debug.Log($"[DroneEnemy] {gameObject.name} disparó desde {spawnPos}");
        Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
    }

    // Dibuja la zona de patrulla en el Editor
    void OnDrawGizmosSelected()
    {
        Vector3 center = Application.isPlaying ? startPos : transform.position;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(center + Vector3.left  * patrolDistance, center + Vector3.right * patrolDistance);
        Gizmos.DrawWireSphere(center + Vector3.left  * patrolDistance, 0.15f);
        Gizmos.DrawWireSphere(center + Vector3.right * patrolDistance, 0.15f);
    }
}
