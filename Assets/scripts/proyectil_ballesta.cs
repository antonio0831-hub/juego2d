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
        // Walls
        int otherMask = 1 << other.gameObject.layer;
        if ((wallsLayer.value & otherMask) != 0)
        {
            Destroy(gameObject);
            return;
        }

        // Si lo dispara el PLAYER, daña ENEMIGOS
        if (fromPlayer)
        {
            if (other.CompareTag("Enemy"))
            {
                vida_enemigo v = other.GetComponentInParent<vida_enemigo>();
                if (v != null) v.TakeDamage(damage);

                Destroy(gameObject);
                return;
            }
        }
        // Si lo dispara el ENEMIGO, daña PLAYER
        else
        {
            if (other.CompareTag("Player"))
            {
                vida v = other.GetComponentInParent<vida>();
                if (v != null) v.TakeDamage(damage);

                Destroy(gameObject);
                return;
            }
        }
    }
}
