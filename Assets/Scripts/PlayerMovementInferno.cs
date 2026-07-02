using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovementInferno : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 14f;

    public float fallMultiplier = 4f;
    public float riseMultiplier = 2.5f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private bool canJump = true;
    private bool gravityInverted = false;

    private float normalGravityScale;

    // Nombres de los parámetros del Animator Controller
    static readonly int ParamIsGrounded = Animator.StringToHash("IsGrounded");
    static readonly int ParamVelocityY  = Animator.StringToHash("VelocityY");

    void Awake()
    {
        rb            = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator      = GetComponent<Animator>();
        normalGravityScale = Mathf.Abs(rb.gravityScale);

        // Friction cero evita que el player se pegue en las esquinas de plataformas
        var col = GetComponent<Collider2D>();
        if (col != null)
        {
            var mat = new PhysicsMaterial2D("PlayerNoFriction") { friction = 0f, bounciness = 0f };
            col.sharedMaterial = mat;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, gravityInverted ? -jumpForce : jumpForce);
            canJump = false;
        }

        // Actualizar Animator
        if (animator != null)
        {
            animator.SetBool(ParamIsGrounded, canJump);
            animator.SetFloat(ParamVelocityY, rb.linearVelocity.y);
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(speed, rb.linearVelocity.y);

        if (!gravityInverted)
        {
            // Esta parte es igual a tu salto original
            if (rb.linearVelocity.y < 0)
            {
                rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
            }
            else if (rb.linearVelocity.y > 0)
            {
                rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (riseMultiplier - 1) * Time.fixedDeltaTime;
            }
        }
        else
        {
            // Misma idea, pero al revés para caminar por el techo
            if (rb.linearVelocity.y > 0)
            {
                rb.linearVelocity += Vector2.up * -Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
            }
            else if (rb.linearVelocity.y < 0)
            {
                rb.linearVelocity += Vector2.up * -Physics2D.gravity.y * (riseMultiplier - 1) * Time.fixedDeltaTime;
            }
        }
    }

    public void SetGravityInverted(bool inverted)
    {
        gravityInverted = inverted;

        if (gravityInverted)
        {
            rb.gravityScale = -normalGravityScale;

            if (spriteRenderer != null)
            {
                spriteRenderer.flipY = true;
            }
        }
        else
        {
            rb.gravityScale = normalGravityScale;

            if (spriteRenderer != null)
            {
                spriteRenderer.flipY = false;
            }
        }

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        canJump = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            // Solo permite saltar si el contacto viene desde abajo (pisando), no desde los lados
            foreach (ContactPoint2D contact in collision.contacts)
            {
                float dot = gravityInverted ? -contact.normal.y : contact.normal.y;
                if (dot > 0.5f)
                {
                    canJump = true;
                    break;
                }
            }
        }

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            // Si la ruleta dio vida extra, absorbe el golpe y no hace nada
            if (CieloBuffManager.Instance != null && CieloBuffManager.Instance.ConsumeExtraLife())
                return;

            CieloScoreManager.RegisterDeath();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}