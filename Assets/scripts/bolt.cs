using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class bolt : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 12f;
    public float maxDistance = 6f;

    [Header("Daño")]
    public int damage = 1;

    [Header("Colisiones")]
    public LayerMask wallsLayer;

    [Header("Quién lo dispara")]
    [SerializeField] private bool fromPlayer = false;

    private Rigidbody2D rb;
    private Vector2 startPosition;
    private bool initialized;

    public void SetOwnerAndTarget(bool isFromPlayer)
    {
        fromPlayer = isFromPlayer;
    }

    public void Init(Vector2 dir)
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = rb.position;

        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.linearVelocity = dir.normalized * speed;

        initialized = true;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.linearVelocity = Vector2.zero;
    }

    void Update()
    {
        if (!initialized) return;

        if (Vector2.Distance(startPosition, rb.position) >= maxDistance)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other) => HandleHit(other);
    void OnCollisionEnter2D(Collision2D col) => HandleHit(col.collider);

    void HandleHit(Collider2D other)
    {
        if (other == null) return;

        // 1) Walls
        int otherMask = 1 << other.gameObject.layer;
        if ((wallsLayer.value & otherMask) != 0)
        {
            Destroy(gameObject);
            return;
        }

        // 2) Si lo dispara el PLAYER: daña ENEMIGOS
        if (fromPlayer)
        {
            // ✅ No dependemos del tag: buscamos vida_enemigo en el objeto o padres
            vida_enemigo enemy = other.GetComponentInParent<vida_enemigo>();
            if (enemy != null && !enemy.IsDead())
            {
                enemy.TakeDamage(damage, true);  // ✅ clave: fromPlayer=true
                Destroy(gameObject);
                return;
            }
        }
        // 3) Si lo dispara el ENEMIGO: daña PLAYER
        else
        {
            vida playerVida = other.GetComponentInParent<vida>();
            if (playerVida != null)
            {
                playerVida.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }
        }

        // 4) Opcional: si choca con algo que no es wall ni objetivo, lo destruimos igual
        // (si no lo quieres, borra estas líneas)
        // Destroy(gameObject);
    }
}
