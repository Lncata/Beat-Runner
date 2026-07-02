using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Proyectil del drone. Cae hacia abajo y mata al player al tocarlo.
/// El GameObject necesita un Collider2D con IsTrigger = true.
/// </summary>
public class DroneProjectile : MonoBehaviour
{
    [Tooltip("Velocidad de caída hacia abajo")]
    public float fallSpeed = 8f;

    [Tooltip("Segundos antes de destruirse si no toca nada")]
    public float lifetime = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Verificar vida extra de la ruleta
            if (CieloBuffManager.Instance != null && CieloBuffManager.Instance.ConsumeExtraLife())
            {
                Destroy(gameObject);
                return;
            }

            CieloScoreManager.RegisterDeath();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        // Se destruye al tocar el suelo u otra superficie
        if (other.CompareTag("Ground"))
            Destroy(gameObject);
    }
}
