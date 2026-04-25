using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    public Animator anim;

    [Header("Level Start")]
    public string level4SceneName = "Level_4";
    public float level4StartX = -6f;

    [Header("Movement")]
    public float speed = 8f;
    float inputMovement;
    bool isRunning;

    [Header("Jumping")]
    public Transform groundCheck;
    public LayerMask groundMask;
    public float radius = 0.15f;
    public float jumpForce = 12f;
    public float doubleJumpForce = 12f;

    bool isOnGround;
    bool canDoubleJump;           // ← This controls whether double jump is available
    bool isDoubleJumping;

    [Header("Dash")]
    public float dashSpeed = 20f;
    public float dashDuration = 3f;
    public float dashCollisionForce = 5f;
    public int dashDamageTaken = 1;
    bool isDashing;
    private float dashDirection;

    [Header("Shooting")]
    public GameObject bullet;
    private int bulletsShotToEnemy = 0;
    private float lastFireTime = -1f;
    private float fireCooldown = 0.5f;
    bool right = true;

    [Header("Player Health")]
    public PlayerHealth playerHealth;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Level 4 position setup
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene.Contains("Level4") || currentScene.Contains("Level 4") || currentScene.Contains("Level_4"))
        {
            transform.position = new Vector3(level4StartX, transform.position.y, transform.position.z);
            Debug.Log($"Player start position set to x={level4StartX} for {currentScene}");
        }

        if (playerHealth == null)
            playerHealth = GetComponent<PlayerHealth>();
    }

    void Update()
    {
        inputMovement = Input.GetAxis("Horizontal");
        isRunning = inputMovement != 0;

        // Ground Check
        isOnGround = Physics2D.OverlapCircle(groundCheck.position, radius, groundMask);

        // Reset double jump when landing
        if (isOnGround)
        {
            canDoubleJump = true;
            isDoubleJumping = false;
        }

        // Jump & Double Jump Logic
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isOnGround)
            {
                Jump(jumpForce);
                canDoubleJump = true;        // Allow double jump after ground jump
            }
            else if (canDoubleJump)
            {
                Jump(doubleJumpForce);
                canDoubleJump = false;       // Disable double jump after using it
                isDoubleJumping = true;
            }
        }

        // Dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing)
        {
            dashDirection = inputMovement != 0 ? Mathf.Sign(inputMovement) : (right ? 1f : -1f);
            if (dashDirection == 0) dashDirection = right ? 1f : -1f;
            StartCoroutine(PerformDash());
        }

        // Shooting
        if (Input.GetMouseButtonDown(0))
        {
            anim.SetTrigger("Shoot");
            StartCoroutine(FireBulletAfterDelay(0.5f)); // adjust delay
        }

        // Animator
        float yVelocity = rb.velocity.y;
        anim.SetFloat("yVelocity", yVelocity);
        anim.SetBool("IsJumping", !isOnGround);
        anim.SetBool("IsDoubleJumping", isDoubleJumping);
        anim.SetBool("IsRunning", isRunning);

        // Flip Logic
        if (inputMovement < 0 && right)
            Flip();
        else if (inputMovement > 0 && !right)
            Flip();
    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            rb.velocity = new Vector2(dashDirection * dashSpeed, rb.velocity.y);
            return;
        }

        rb.velocity = new Vector2(inputMovement * speed, rb.velocity.y);
    }

    void Jump(float force)
    {
        rb.velocity = new Vector2(rb.velocity.x, force);
    }

    IEnumerator PerformDash()
    {
        isDashing = true;
        anim.SetBool("IsDashing", true);

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(dashDirection * dashSpeed, 0f);

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        isDashing = false;
        anim.SetBool("IsDashing", false);

        if (!isOnGround)
            rb.velocity = new Vector2(rb.velocity.x, -2f);
    }

    void Flip()
    {
        right = !right;
        GetComponentInChildren<SpriteRenderer>().flipX = !right;
    }

    // ... (keep your FireBullet and OnCollisionEnter2D as they are)
    public void FireBullet()
    {
        if (Time.time - lastFireTime < fireCooldown)
            return;

        lastFireTime = Time.time;

        float offsetX = right ? 1.5f : -1.5f;
        Vector3 spawnPos = transform.position + new Vector3(offsetX, 0f, 0f);

        GameObject newBullet = Instantiate(bullet, spawnPos, Quaternion.identity);

        bulletScript bs = newBullet.GetComponent<bulletScript>();
        if (bs != null)
        {
            bs.speed = 15f;
            bs.shootDirection = right ? 1 : -1;
        }

        bulletsShotToEnemy++;
    }

    IEnumerator FireBulletAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        FireBullet();
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDashing && collision.collider.CompareTag("Enemy"))
        {
            Rigidbody2D enemyRb = collision.collider.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
                enemyRb.AddForce(new Vector2(dashDirection * dashCollisionForce, 0), ForceMode2D.Impulse);

            if (playerHealth != null)
                playerHealth.TakeDamage(dashDamageTaken);
        }
    }
}