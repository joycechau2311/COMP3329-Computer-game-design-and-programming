using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    public Animator anim;

    [Header("Movement")]
    public float speed;
    float inputMovement;
    bool isRunning;

    [Header("Jumping")]
    public Transform groundCheck;
    public LayerMask groundMask;
    public float radius = 0.15f;
    public float jumpForce = 12f;
    public float doubleJumpForce = 12f;
    bool isOnGround;
    bool canDoubleJump;
    bool isDoubleJumping;

    [Header("Dash")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    bool isDashing;

    [Header("Shooting")]
    public GameObject bullet;
    bool right = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // -------------------------------------------
        // Movement Input
        // -------------------------------------------
        inputMovement = Input.GetAxis("Horizontal");
        isRunning = inputMovement != 0;

        // -------------------------------------------
        // Ground Check
        // -------------------------------------------
        isOnGround = Physics2D.OverlapCircle(groundCheck.position, radius, groundMask);
        if (isOnGround)
            isDoubleJumping = false;

        // -------------------------------------------
        // Jump & Double Jump
        // -------------------------------------------
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isOnGround)
            {
                Jump(jumpForce);
                canDoubleJump = true;
            }
            else if (canDoubleJump)
            {
                Jump(doubleJumpForce);
                canDoubleJump = false;
                isDoubleJumping = true;
            }
        }

        // -------------------------------------------
        // Dash
        // -------------------------------------------
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing)
        {
            StartCoroutine(PerformDash());
        }

        // -------------------------------------------
        // Shooting
        // -------------------------------------------
        if (Input.GetMouseButtonDown(0))
        {
            anim.SetTrigger("Shoot");
            // bullet is fired through animation event now
        }

        // -------------------------------------------
        // Animator
        // -------------------------------------------
        anim.SetBool("IsJumping", !isOnGround);
        anim.SetBool("IsDoubleJumping", isDoubleJumping);
        anim.SetBool("IsRunning", isRunning);
        //anim.SetFloat("yVelocity", rb.velocity.y);

        // -------------------------------------------
        // Flip sprite
        // -------------------------------------------
        if (!right && inputMovement > 0) Flip();
        else if (right && inputMovement < 0) Flip();
    }

    void FixedUpdate()
    {
        if (isDashing) return;

        rb.velocity = new Vector2(inputMovement * speed, rb.velocity.y);
    }

    // -------------------------------------------
    // Jump
    // -------------------------------------------
    void Jump(float force)
    {
        rb.velocity = new Vector2(rb.velocity.x, force);
    }

    // -------------------------------------------
    // Dash
    // -------------------------------------------
    IEnumerator PerformDash()
    {
        isDashing = true;
        anim.SetBool("IsDashing", true);

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;

        float dashDir = right ? 1 : -1;
        rb.velocity = new Vector2(dashDir * dashSpeed, 0);

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        isDashing = false;
        anim.SetBool("IsDashing", false);
    }

    // -------------------------------------------
    // Flip
    // -------------------------------------------
    void Flip()
    {
        right = !right;
        GetComponentInChildren<SpriteRenderer>().flipX = !right;
    }

    // -------------------------------------------
    // Animation Event: FIRE BULLET HERE
    // -------------------------------------------
    public void FireBullet()
    {
        float offsetX = 1f;
        Vector3 spawnPos =
            transform.position +
            new Vector3(offsetX * (right ? 1 : -1), -0.2f, 0);

        GameObject newBullet = Instantiate(bullet, spawnPos, Quaternion.identity);

        bulletScript bs = newBullet.GetComponent<bulletScript>();
        if (bs)
            bs.shootDirection = right ? 1 : -1;
    }
}