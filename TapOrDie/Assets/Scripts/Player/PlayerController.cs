using UnityEngine;
using TapOrDie.Core;
using TapOrDie.Audio;

namespace TapOrDie.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CircleCollider2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Jump Settings")]
        [SerializeField] private float jumpForce = 10f;
        [SerializeField] private float maxJumpVelocity = 12f;
        [SerializeField] private float fallMultiplier = 2.5f;
        [SerializeField] private float lowJumpMultiplier = 2f;

        [Header("Ground Check")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundCheckRadius = 0.2f;
        [SerializeField] private LayerMask groundLayer;

        [Header("Visual")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private TrailRenderer trailRenderer;
        [SerializeField] private ParticleSystem jumpParticles;
        [SerializeField] private ParticleSystem deathParticles;

        private Rigidbody2D rb;
        private CircleCollider2D circleCollider;
        private bool isGrounded;
        private bool canJump = true;
        private bool isDead;
        private Vector3 startPosition;

        public bool IsGrounded => isGrounded;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            circleCollider = GetComponent<CircleCollider2D>();

            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();

            startPosition = transform.position;
        }

        private void Start()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStart += OnGameStart;
                GameManager.Instance.OnExtraLifeUsed += OnExtraLifeUsed;
            }
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStart -= OnGameStart;
                GameManager.Instance.OnExtraLifeUsed -= OnExtraLifeUsed;
            }
        }

        private void Update()
        {
            if (isDead || GameManager.Instance == null) return;
            if (!GameManager.Instance.IsPlaying || GameManager.Instance.IsPaused) return;

            CheckGrounded();
            HandleInput();
        }

        private void FixedUpdate()
        {
            if (isDead || GameManager.Instance == null) return;
            if (!GameManager.Instance.IsPlaying || GameManager.Instance.IsPaused) return;

            ApplyBetterJumpPhysics();
            MoveForward();
        }

        private void CheckGrounded()
        {
            if (groundCheck != null)
            {
                isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
            }
            else
            {
                // Fallback: raycast down
                isGrounded = Physics2D.Raycast(transform.position, Vector2.down,
                    circleCollider.radius + 0.1f, groundLayer);
            }
        }

        private void HandleInput()
        {
            bool jumpPressed = Input.GetMouseButtonDown(0) ||
                              Input.GetKeyDown(KeyCode.Space) ||
                              Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;

            if (jumpPressed && canJump)
            {
                Jump();
            }
        }

        private void Jump()
        {
            // Reset vertical velocity for consistent jumps
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            // Clamp velocity
            if (rb.velocity.y > maxJumpVelocity)
            {
                rb.velocity = new Vector2(rb.velocity.x, maxJumpVelocity);
            }

            // Effects
            if (jumpParticles != null)
                jumpParticles.Play();

            AudioManager.Instance?.PlayJumpSound();

            // Visual feedback
            if (spriteRenderer != null)
            {
                StartCoroutine(JumpSquash());
            }
        }

        private System.Collections.IEnumerator JumpSquash()
        {
            Vector3 originalScale = transform.localScale;
            Vector3 squashScale = new Vector3(originalScale.x * 1.2f, originalScale.y * 0.8f, originalScale.z);

            transform.localScale = squashScale;
            yield return new WaitForSeconds(0.1f);

            float elapsed = 0f;
            float duration = 0.15f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                transform.localScale = Vector3.Lerp(squashScale, originalScale, elapsed / duration);
                yield return null;
            }
            transform.localScale = originalScale;
        }

        private void ApplyBetterJumpPhysics()
        {
            if (rb.velocity.y < 0)
            {
                // Falling - apply extra gravity
                rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
            }
            else if (rb.velocity.y > 0 && !Input.GetMouseButton(0) && !Input.GetKey(KeyCode.Space))
            {
                // Rising but not holding jump - apply extra gravity for shorter jump
                rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
            }
        }

        private void MoveForward()
        {
            // Player doesn't actually move - the world scrolls towards them
            // This is handled by ObstacleManager moving obstacles
            // But we can add subtle horizontal movement if needed
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (isDead) return;

            if (collision.gameObject.CompareTag("Obstacle"))
            {
                Die();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (isDead) return;

            if (other.CompareTag("Obstacle"))
            {
                Die();
            }
            else if (other.CompareTag("Collectible"))
            {
                CollectItem(other.gameObject);
            }
        }

        private void Die()
        {
            if (isDead) return;

            // Check for extra life
            if (GameManager.Instance != null && GameManager.Instance.HasExtraLife)
            {
                GameManager.Instance.UseExtraLife();
                return;
            }

            isDead = true;
            canJump = false;

            // Effects
            if (deathParticles != null)
            {
                deathParticles.transform.SetParent(null);
                deathParticles.Play();
            }

            if (trailRenderer != null)
                trailRenderer.enabled = false;

            AudioManager.Instance?.PlayHitSound();

            // Hide player
            if (spriteRenderer != null)
                spriteRenderer.enabled = false;

            rb.simulated = false;

            // Trigger game over
            GameManager.Instance?.GameOver();
        }

        private void CollectItem(GameObject item)
        {
            AudioManager.Instance?.PlayCollectSound();
            GameManager.Instance?.AddBonusScore(5);
            item.SetActive(false);
        }

        private void OnGameStart()
        {
            ResetPlayer();
        }

        private void OnExtraLifeUsed()
        {
            // Brief invincibility or reset position slightly
            StartCoroutine(InvincibilityFrames());
        }

        private System.Collections.IEnumerator InvincibilityFrames()
        {
            canJump = true;

            // Flash effect
            for (int i = 0; i < 6; i++)
            {
                if (spriteRenderer != null)
                    spriteRenderer.enabled = !spriteRenderer.enabled;
                yield return new WaitForSeconds(0.1f);
            }

            if (spriteRenderer != null)
                spriteRenderer.enabled = true;
        }

        public void ResetPlayer()
        {
            isDead = false;
            canJump = true;
            transform.position = startPosition;
            rb.velocity = Vector2.zero;
            rb.simulated = true;

            if (spriteRenderer != null)
                spriteRenderer.enabled = true;

            if (trailRenderer != null)
                trailRenderer.enabled = true;

            transform.localScale = Vector3.one;
        }

        private void OnDrawGizmosSelected()
        {
            if (groundCheck != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
            }
        }
    }
}
