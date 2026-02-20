using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ia_enemigo_tierra : MonoBehaviour
{
    public enum State { Patrol, Chase, Attack, WaitAtPoint }

    [Header("Movimiento (Patrulla)")]
    public float moveSpeed = 2f;
    public Transform pointA;
    public Transform pointB;
    public float arriveDistance = 0.2f;
    [Header("Sonido de Pasos")]
    public AudioSource audioSourcePasos;
    public AudioClip sonidoPaso;
    public float tiempoEntrePasos = 0.4f;
    private float stepTimer;
    [Header("Pausa en puntos")]
    public float waitAtPatrolPoint = 1f;

    [Header("Jugador")]
    public string playerTag = "Player";

    [Header("Detección")]
    public float detectionRange = 6f;

    [Header("Ataque")]
    public float attackRange = 3f; // Para rango, asegúrate que sea menor que detectionRange
    public float attackCooldown = 0.8f;
    public float attackAnimDuration = 0.45f;
    public float idleAfterAttack = 0.25f;

    [Header("Ataques (Componentes)")]
    public ataque_distancia ataqueDistancia;
    public melee_universal meleeAttack;

    [Header("Visión (Cono)")]
    [Range(0f, 360f)] public float viewAngle = 90f;
    [Tooltip("¿Tu sprite original mira a la derecha?")]
    public bool spriteDefaultFacesRight = true;

    [Header("Obstáculos")]
    public LayerMask obstacleLayer; 

    [Header("Animación")]
    public string isWalkingBool = "isWalking";
    public string attackTrigger = "ataque";

    [Header("Flip")]
    public bool useFlipX = true;

    [Header("Gizmos Visuales")]
    public bool drawGizmos = true;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;
    private Transform player;

    private Vector2 aPos, bPos;
    private Vector2 currentPatrolTarget;
    private State state = State.Patrol;
    private bool attackingLoop = false;
    private bool isWalking = false;
    private Coroutine waitRoutine;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        rb.freezeRotation = true;
    }

    void Start()
    {
        if (pointA == null || pointB == null)
        {
            Debug.LogError("Asigna PointA y PointB en el Inspector.");
            enabled = false;
            return;
        }

        aPos = pointA.position;
        bPos = pointB.position;
        currentPatrolTarget = bPos;

        if (ataqueDistancia == null) ataqueDistancia = GetComponent<ataque_distancia>();
        if (meleeAttack == null) meleeAttack = GetComponent<melee_universal>();

        if (ataqueDistancia != null) ataqueDistancia.enabled = false;

        FindPlayer();
        state = State.Patrol;
    }

    void Update()
    {
        if (player == null) { FindPlayer(); return; }
        if (attackingLoop) return;

        bool canSee = CanSeePlayer(out float dist, detectionRange);

        if (canSee)
        {
            StopWaiting();
            if (dist <= attackRange)
            {
                StartAttack();
            }
            else
            {
                state = State.Chase;
            }
        }
        else
        {
            if (state == State.Chase || state == State.Attack)
                state = State.Patrol;
        }
    }

    void FixedUpdate()
    {
        if (attackingLoop) return;

        switch (state)
        {
            case State.Patrol: PatrolMove(); break;
            case State.Chase: ChaseMove(); break;
            case State.WaitAtPoint: rb.linearVelocity = Vector2.zero; break;
        }
    }

    // --- LÓGICA DE MOVIMIENTO ---

    void PatrolMove()
    {
        Vector2 target = new Vector2(currentPatrolTarget.x, rb.position.y);
        MoveTowardsTarget(target);

        if (Vector2.Distance(rb.position, target) <= arriveDistance)
        {
            if (waitRoutine == null) waitRoutine = StartCoroutine(WaitThenSwitchPoint());
        }
    }

    void ChaseMove()
    {
        if (player == null) return;
        Vector2 target = new Vector2(player.position.x, rb.position.y);
        MoveTowardsTarget(target);
    }

    void MoveTowardsTarget(Vector2 target)
    {
        Vector2 next = Vector2.MoveTowards(rb.position, target, moveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(next);

        float dx = target.x - rb.position.x;
        UpdateFlip(dx);
        SetWalking(true);
        stepTimer -= Time.deltaTime;
    if (stepTimer <= 0f)
    {
        if (audioSourcePasos != null && sonidoPaso != null)
        {
            audioSourcePasos.PlayOneShot(sonidoPaso);
        }
        stepTimer = tiempoEntrePasos; // Reinicia el cronómetro
    }
    }

    // --- LÓGICA DE ATAQUE ---

    void StartAttack()
    {
        if (!attackingLoop) StartCoroutine(AttackLoop());
    }

    IEnumerator AttackLoop()
    {
        attackingLoop = true;
        state = State.Attack;
        SetWalking(false);

        if (ataqueDistancia != null) ataqueDistancia.enabled = true;

        while (CanSeePlayer(out float dist, attackRange))
        {
            // Re-orientar hacia el jugador antes de disparar
            UpdateFlip(player.position.x - transform.position.x);

            if (meleeAttack != null) meleeAttack.DoAttack();
            else if (ataqueDistancia != null) ataqueDistancia.Disparar(player);

            if (anim != null) anim.SetTrigger(attackTrigger);

            yield return new WaitForSeconds(attackAnimDuration + idleAfterAttack + attackCooldown);
        }

        if (ataqueDistancia != null) ataqueDistancia.enabled = false;
        attackingLoop = false;
        state = State.Patrol;
    }

    // --- VISIÓN Y FLIP (PUNTOS CLAVE) ---

    bool CanSeePlayer(out float dist, float maxRange)
    {
        dist = Mathf.Infinity;
        if (player == null) return false;

        dist = Vector2.Distance(transform.position, player.position);
        if (dist > maxRange) return false;

        // Comprobar ángulo
        Vector2 dirToPlayer = (player.position - transform.position).normalized;
        float angle = Vector2.Angle(GetForward2D(), dirToPlayer);
        if (angle > viewAngle * 0.5f) return false;

        // Comprobar obstáculos
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dirToPlayer, dist, obstacleLayer);
        return hit.collider == null;
    }

    Vector2 GetForward2D()
    {
        // Esta es la corrección principal:
        if (sr == null) return transform.right;
        
        // Si el sprite NO tiene flip, mira hacia su lado original.
        // Si tiene flip, mira al lado opuesto.
        Vector2 facing = spriteDefaultFacesRight ? Vector2.right : Vector2.left;
        return sr.flipX ? -facing : facing;
    }

    void UpdateFlip(float dx)
    {
        if (!useFlipX || sr == null || Mathf.Abs(dx) < 0.05f) return;

        // Si dx > 0 (derecha) y el sprite mira originalmente a la derecha, flipX = false
        if (spriteDefaultFacesRight)
            sr.flipX = (dx < 0);
        else
            sr.flipX = (dx > 0);
    }

    // --- UTILIDADES ---

    void SetWalking(bool walking)
    {
        isWalking = walking;
        if (anim != null) anim.SetBool(isWalkingBool, walking);
    }

    IEnumerator WaitThenSwitchPoint()
    {
        state = State.WaitAtPoint;
        SetWalking(false);
        yield return new WaitForSeconds(waitAtPatrolPoint);
        currentPatrolTarget = (currentPatrolTarget == aPos) ? bPos : aPos;
        state = State.Patrol;
        waitRoutine = null;
    }

    void StopWaiting()
    {
        if (waitRoutine != null) { StopCoroutine(waitRoutine); waitRoutine = null; }
    }

    void FindPlayer()
    {
        GameObject p = GameObject.FindGameObjectWithTag(playerTag);
        if (p != null) player = p.transform;
    }

    void OnDrawGizmosSelected()
    {
        if (!drawGizmos) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Vector2 forward = GetForward2D();
        Gizmos.color = Color.yellow;
        Vector3 leftRay = Quaternion.Euler(0, 0, viewAngle * 0.5f) * forward;
        Vector3 rightRay = Quaternion.Euler(0, 0, -viewAngle * 0.5f) * forward;
        Gizmos.DrawLine(transform.position, transform.position + leftRay * detectionRange);
        Gizmos.DrawLine(transform.position, transform.position + rightRay * detectionRange);
    }
}