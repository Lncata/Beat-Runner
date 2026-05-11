using UnityEngine;

public class PlayerFormSwitcher : MonoBehaviour
{
    public Sprite normalSprite;
    public Sprite catSprite;

    public RuntimeAnimatorController normalAnimator;
    public RuntimeAnimatorController catAnimator;

    public Vector2 normalColliderSize = new Vector2(1.34f, 3.76f);
    public Vector2 normalColliderOffset = new Vector2(0.02f, -0.01f);

    public Vector2 catColliderSize = new Vector2(1.2f, 0.8f);
    public Vector2 catColliderOffset = new Vector2(0f, -0.2f);

    public float normalJumpForce = 14f;
    public float catJumpForce = 10f;

    public float normalFallMultiplier = 4.5f;
    public float catFallMultiplier = 5.5f;

    public float normalRiseMultiplier = 2.5f;
    public float catRiseMultiplier = 2.2f;

    public Vector3 normalScale = new Vector3(0.8461f, 0.8829f, 1f);
    public Vector3 catScale = new Vector3(2.2f, 2.2f, 1f);  

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private BoxCollider2D boxCollider;
    private PlayerMovementInferno movement;

    private bool isCat = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        movement = GetComponent<PlayerMovementInferno>();
    }

    public void TransformToCat()
    {
        if (isCat) return;

        isCat = true;

        transform.localScale = catScale;

        if (spriteRenderer != null && catSprite != null)
        {
            spriteRenderer.sprite = catSprite;
        }

        if (animator != null && catAnimator != null)
        {
            animator.runtimeAnimatorController = catAnimator;
        }

        if (boxCollider != null)
        {
            boxCollider.size = catColliderSize;
            boxCollider.offset = catColliderOffset;
        }

        if (movement != null)
        {
            movement.jumpForce = catJumpForce;
            movement.fallMultiplier = catFallMultiplier;
            movement.riseMultiplier = catRiseMultiplier;
        }
    }

    public void TransformToNormal()
    {
        if (!isCat) return;

        isCat = false;

        transform.localScale = normalScale;

        if (spriteRenderer != null && normalSprite != null)
        {
            spriteRenderer.sprite = normalSprite;
        }

        if (animator != null && normalAnimator != null)
        {
            animator.runtimeAnimatorController = normalAnimator;
        }

        if (boxCollider != null)
        {
            boxCollider.size = normalColliderSize;
            boxCollider.offset = normalColliderOffset;
        }

        if (movement != null)
        {
            movement.jumpForce = normalJumpForce;
            movement.fallMultiplier = normalFallMultiplier;
            movement.riseMultiplier = normalRiseMultiplier;
        }
    }
}