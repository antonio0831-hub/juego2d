using UnityEngine;

public class movimiento : MonoBehaviour
{
    private Rigidbody2D rb;
    private float Horizontal;

    [Header("Movimiento")]
    public float Speed = 5f;
    public float velocidadNormal;
    public float JumpForce = 8f;
    [Header("Sonidos")]
    public AudioSource pasos;
    public AudioSource salto;
    public AudioClip sonidosalto;
    public AudioSource sprint;

    [Header("Suelo (Ground Check)")]
    public bool Grounded;
    public Transform groundCheck;              // Empty bajo los pies
    public float groundCheckRadius = 0.15f;    // pequeño
    public LayerMask groundLayer;              // capa suelo

    [Header("Wall Detection")]
    public Transform wallCheck;                // Empty al lateral
    public float wallCheckDistance = 0.25f;
    public LayerMask wallLayer;                // capa "walls"
    public bool TouchingWall;

    [Header("Animator")]
    private Animator anim;

    [Header("Debug")]
    public bool debug = true;

    private salto_pared wallScript;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        wallScript = GetComponent<salto_pared>(); // ✅ importante

        velocidadNormal = Speed;
    }

    void Update()
    {
        Horizontal = Input.GetAxisRaw("Horizontal");
        anim.SetBool("andar", Horizontal != 0.0f);
    if (Horizontal != 0.0f) 
    {
    // ...y el sonido NO está sonando, lo activamos
    if (!pasos.isPlaying) 
    {
        pasos.Play();
    }
    }
    else // SI NO hay movimiento, lo paramos
    {
        pasos.Stop();
    }
    
     // Detiene el sonido si el jugador deja de moverse
    
        // Flip
        if (Horizontal != 0)
            transform.localScale = new Vector3(Mathf.Sign(Horizontal), 1, 1);

        // ✅ Grounded bien (solo suelo)
        if (groundCheck != null)
            Grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        else
            Grounded = false;

        // ✅ Wall detection
        TouchingWall = CheckWall();

        // ✅ PASAR DATOS AL SCRIPT salto_pared (CLAVE)
        if (wallScript != null)
        {
            int facingDir = (transform.localScale.x >= 0) ? 1 : -1;
            wallScript.SetWallState(TouchingWall, facingDir, Grounded);
        }
        else
        {
            if (debug) Debug.LogWarning("No hay script 'salto_pared' en el Player.");
        }

        if (debug)
            Debug.Log($"Grounded={Grounded}  TouchingWall={TouchingWall}");

        // Salto normal SOLO si grounded y NO estás pegado
        if (Input.GetKeyDown(KeyCode.Space) && Grounded && (wallScript == null || !wallScript.IsClinging()))
        {
            salto.PlayOneShot(sonidosalto);
            anim.SetTrigger("salto");
            rb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
        }

        if (Input.GetMouseButtonDown(0))
            anim.SetTrigger("ataque");

        if (Input.GetKey(KeyCode.LeftShift) && Grounded)
            Speed = 1f;
        else
            Speed = velocidadNormal;
    if (Input.GetKey(KeyCode.LeftShift)) 
    {
    // Solo le damos al Play si NO está sonando ya
    if (!sprint.isPlaying) 
    {
        sprint.Play();
        pasos.Stop();
    }
    }
    else 
    {
    // Si soltamos el Shift, paramos el sonido
    sprint.Stop();
    }
    }

    void FixedUpdate()
    {
        // ✅ si estás pegado, NO sobrescribas la velocidad
        if (wallScript != null && wallScript.IsClinging())
            return;

        rb.linearVelocity = new Vector2(Horizontal * Speed, rb.linearVelocity.y);
    }

    bool CheckWall()
    {
        if (wallCheck == null)
        {
            if (debug) Debug.LogWarning("wallCheck NO asignado");
            return false;
        }

        Vector2 dir = (transform.localScale.x >= 0) ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(wallCheck.position, dir, wallCheckDistance, wallLayer);

        if (debug)
            Debug.DrawRay(wallCheck.position, dir * wallCheckDistance, hit.collider ? Color.red : Color.green);

        return hit.collider != null;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        if (wallCheck != null)
        {
            Gizmos.color = Color.cyan;
            Vector2 dir = (transform.localScale.x >= 0) ? Vector2.right : Vector2.left;
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + (Vector3)(dir * wallCheckDistance));
        }
    }
}
