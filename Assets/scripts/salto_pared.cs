using UnityEngine;

public class salto_pared : MonoBehaviour
{
    [Header("Teclas")]
    public KeyCode clingKey = KeyCode.E;
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Salto desde pared")]
    public float jumpForceX = 8f;
    public float jumpForceY = 10f;

    [Header("Pegado")]
    public float clingGravity = 0f;

    [Header("Animator")]
    public Animator anim;
    public string clingBool = "pegado";
    public string wallJumpTrigger = "salto_pared";

    private Rigidbody2D rb;

    private bool touchingWall;
    private bool grounded;
    private int facingDir = 1;

    private bool clinging;

    private float originalGravity;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (anim == null) anim = GetComponent<Animator>();

        originalGravity = rb.gravityScale;
    }

    // ← llamado desde movimiento.cs
    public void SetWallState(bool isTouchingWall, int currentFacingDir, bool isGrounded)
    {
        touchingWall = isTouchingWall;
        facingDir = currentFacingDir;
        grounded = isGrounded;

        // Si toca suelo → nunca pegado
        if (grounded)
            StopCling();
    }

    void Update()
    {
        HandleClingToggle();
        HandleWallJump();
    }

    void HandleClingToggle()
    {
        if (!touchingWall || grounded) return;

        // ✅ Toggle real (NO mantener tecla)
        if (Input.GetKeyDown(clingKey))
        {
            if (clinging)
                StopCling();
            else
                StartCling();
        }
    }

    void StartCling()
    {
        clinging = true;

        rb.gravityScale = clingGravity;
        rb.linearVelocity = Vector2.zero;

        if (anim != null)
            anim.SetBool(clingBool, true);
    }

    void StopCling()
    {
        if (!clinging) return;

        clinging = false;

        rb.gravityScale = originalGravity;

        if (anim != null)
            anim.SetBool(clingBool, false);
    }

    void HandleWallJump()
    {
        if (!clinging) return;

        if (Input.GetKeyDown(jumpKey))
        {
            StopCling();

            float dir = -facingDir;

            rb.linearVelocity = Vector2.zero;
            rb.AddForce(new Vector2(dir * jumpForceX, jumpForceY), ForceMode2D.Impulse);

            if (anim != null && !string.IsNullOrEmpty(wallJumpTrigger))
            {
                anim.ResetTrigger(wallJumpTrigger);
                anim.SetTrigger(wallJumpTrigger);
            }
        }
    }

    public bool IsClinging()
    {
        return clinging;
    }
}
