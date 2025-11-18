using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float jumpForce = 10f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    [SerializeField] private bool isGrounded = false;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool canTakeDamage = true;
    private float damageCooldown = 0.3f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (GameManagerMiniGame1.Instance.isGameWin || GameManagerMiniGame1.Instance.isGameOver)
        {
            animator.SetBool("IsRunning", false);
            return;
        }

        if (!GameManagerMiniGame1.Instance.isGameStart)
        {
            animator.SetBool("IsRunning", false);
            return;
        }

        animator.SetBool("IsRunning", true);

        if (Input.GetMouseButtonDown(0) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isGrounded = false;
            animator.SetBool("IsJump", true);
        }

        if (rb.velocity.y < 0)
        {
            rb.velocity += (fallMultiplier - 1) * Physics2D.gravity.y * Time.deltaTime * Vector2.up;
        }
        else if (rb.velocity.y > 0 && !Input.GetMouseButton(0))
        {
            rb.velocity += (lowJumpMultiplier - 1) * Physics2D.gravity.y * Time.deltaTime * Vector2.up;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            animator.SetBool("IsJump", false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!GameManagerMiniGame1.Instance.isGameStart) return;

        if (collision.CompareTag("Spike"))
        {
            TryTakeDamage();
        }
    }

    private void TryTakeDamage()
    {
        if (!canTakeDamage) return;

        canTakeDamage = false;
        TakeDamage();
        Invoke(nameof(ResetDamage), damageCooldown);
    }

    private void ResetDamage()
    {
        canTakeDamage = true;
    }

    private void TakeDamage()
    {
        if (GameManagerMiniGame1.Instance.isGameOver ||
            GameManagerMiniGame1.Instance.isGameWin ||
            !GameManagerMiniGame1.Instance.isGameStart)  
            return;

        HitFlash();
        SoundManager.Instance.PlaySFX("Hurt");
        GameManagerMiniGame1.Instance.LoseHeart();

        if (GameManagerMiniGame1.Instance.isGameOver)
        {
            animator.SetTrigger("isDead");
        }
    }

    private void HitFlash()
    {
        spriteRenderer.DOKill();

        spriteRenderer.DOColor(Color.red, 0.1f)
            .SetLoops(6, LoopType.Yoyo)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                spriteRenderer.color = Color.white;
            });
    }
}
