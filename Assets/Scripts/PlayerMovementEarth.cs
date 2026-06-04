using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovementEarth: MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 14f;

    public float fallMultiplier = 4f;
    public float riseMultiplier = 2.5f;

    private Rigidbody2D rb;
    private bool canJump = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space) && canJump)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            canJump = false;
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(speed, rb.linearVelocity.y);

        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (riseMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            canJump = true;
        }

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("¡Colisión con obstáculo detectada! Recargando escena...");
            Time.timeScale = 1f; // Asegurar que el tiempo corre normalmente
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}