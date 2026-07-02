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

    static readonly int ParamIsGrounded = Animator.StringToHash("IsGrounded");
    static readonly int ParamVelocityY  = Animator.StringToHash("VelocityY");

    void Awake()
    {
        rb            = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator      = GetComponent<Animator>();
        normalGravityScale = Mathf.Abs(rb.gravityScale);

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
            if (rb.linearVelocity.y < 0)
                rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
            else if (rb.linearVelocity.y > 0)
                rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (riseMultiplier - 1) * Time.fixedDeltaTime;
        }
        else
        {
            if (rb.linearVelocity.y > 0)
                rb.linearVelocity += Vector2.up * -Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
            else if (rb.linearVelocity.y < 0)
                rb.linearVelocity += Vector2.up * -Physics2D.gravity.y * (riseMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    public void SetGravityInverted(bool inverted)
    {
        gravityInverted = inverted;

        if (gravityInverted)
        {
            rb.gravityScale = -normalGravityScale;
            if (spriteRenderer != null) spriteRenderer.flipY = true;
        }
        else
        {
            rb.gravityScale = normalGravityScale;
            if (spriteRenderer != null) spriteRenderer.flipY = false;
        }

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        canJump = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                float dot = gravityInverted ? -contact.normal.y : contact.normal.y;
                if (dot > 0.5f) { canJump = true; break; }
            }
        }

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            if (CieloBuffManager.Instance != null && CieloBuffManager.Instance.ConsumeExtraLife())
                return;

            CieloScoreManager.RegisterDeath();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
