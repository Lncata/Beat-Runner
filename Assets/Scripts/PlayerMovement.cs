using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
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

        // ---------- BUFF SPEED ----------
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.selectedBuff == BuffType.SpeedMultiplier)
            {
                speed *= 1.5f;

                Debug.Log("BUFF ACTIVADO: SPEED x1.5");
            }

            // ---------- BUFF MINI RUNNER ----------
            if (GameManager.Instance.selectedBuff == BuffType.MiniRunner)
            {
                transform.localScale = new Vector3(0.5f, 0.5f, 1);

                Debug.Log("BUFF ACTIVADO: MINI RUNNER");
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            rb.linearVelocity =
                new Vector2(rb.linearVelocity.x, jumpForce);

            canJump = false;
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity =
            new Vector2(speed, rb.linearVelocity.y);

        // ---------- CAÍDA MÁS RÁPIDA ----------
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity +=
                Vector2.up *
                Physics2D.gravity.y *
                (fallMultiplier - 1) *
                Time.fixedDeltaTime;
        }

        // ---------- SUBIDA MÁS NATURAL ----------
        else if (rb.linearVelocity.y > 0)
        {
            rb.linearVelocity +=
                Vector2.up *
                Physics2D.gravity.y *
                (riseMultiplier - 1) *
                Time.fixedDeltaTime;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ---------- TOCAR SUELO ----------
        if (collision.gameObject.CompareTag("Ground"))
        {
            canJump = true;
        }

        // ---------- MORIR ----------
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            SceneManager.LoadScene(
                SceneManager.GetActiveScene().name);
        }
    }
}