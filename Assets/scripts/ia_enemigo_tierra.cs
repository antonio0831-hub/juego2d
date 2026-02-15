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
    public float arriveDistance = 0.12f;

    [Header("Pausa en puntos (manteniendo ANDAR)")]
    public float waitAtPatrolPoint = 1f;

    [Header("Jugador")]
    public string playerTag = "Player";

    [Header("Detección (ver al jugador)")]
    public float detectionRange = 6f;

    [Header("Ataque (rango)")]
    public float attackRange = 1.2f;          // ✅ melee típico ~1-1.5 / distancia 3-5
    public float attackCooldown = 0.8f;
    public float attackAnimDuration = 0.45f;
    public float idleAfterAttack = 0.25f;

    [Header("Ataque a distancia (opcional)")]
    public ataque_distancia ataqueDistancia;

    [Header("Ataque melee (opcional)")]
    public melee_universal meleeAttack;       // ✅ arrástralo si es enemigo melee

    [Header("Visión (cono delante)")]
    [Range(0f, 180f)] public float viewAngle = 90f;
    public bool spriteDefaultFacesRight = true;

    [Header("Paredes (NO atraviesa)")]
    public LayerMask obstacleLayer; // "walls"

    [Header("Animator")]
    public string isWalkingBool = "isWalking";
    public string attackTrigger = "ataque";

    [Header("Flip")]
    public bool useFlipX = true;

    [Header("Gizmos")]
    public bool drawGizmos = true;
    public bool drawCone = true;
    public bool drawLineToPlayer = true;

    // ---- privados ----
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
            Debug.LogError("Faltan pointA / pointB en el Inspector.");
            enabled = false;
            return;
        }

        aPos = pointA.position;
        bPos = pointB.position;
        currentPatrolTarget = bPos;

        if (ataqueDistancia == null)
            ataqueDistancia = GetComponent<ataque_distancia>();

        if (meleeAttack == null)
            meleeAttack = GetComponent<melee_universal>();

        // si existen, desactiva “ataque” por defecto (la IA los activa cuando toca)
        if (ataqueDistancia != null) ataqueDistancia.enabled = false;

        FindPlayer();
        SetWalking(true);
        state = State.Patrol;
    }

    void Update()
    {
        if (player == null) FindPlayer();
        if (attackingLoop) return;

        // si esperamos en punto pero vemos player, cancelamos espera y reaccionamos
        if (state == State.WaitAtPoint && player != null)
        {
            if (CanSeePlayer(out float dist, detectionRange))
            {
                StopWaiting();

                if (dist <= attackRange && CanSeePlayer(out _, attackRange))
                    StartAttack();
                else
                    state = State.Chase;

                return;
            }
        }

        // Lógica principal
        if (player != null && CanSeePlayer(out float distanceToPlayer, detectionRange))
        {
            if (distanceToPlayer <= attackRange && CanSeePlayer(out _, attackRange))
                StartAttack();
            else
                state = State.Chase; // ✅ ve pero no llega -> persigue
        }
        else
        {
            if (state == State.Chase) state = State.Patrol;
            if (state != State.WaitAtPoint) state = State.Patrol;
        }
    }

    void FixedUpdate()
    {
        if (attackingLoop) return;

        switch (state)
        {
            case State.Patrol: PatrolMove(); break;
            case State.Chase:  ChaseMove();  break;
            case State.WaitAtPoint: /* quieto */ break;
        }
    }

    // ------------------ PATRULLA ------------------
    void PatrolMove()
    {
        Vector2 pos = rb.position;
        Vector2 target = new Vector2(currentPatrolTarget.x, pos.y);

        Vector2 next = Vector2.MoveTowards(pos, target, moveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(next);

        float dx = target.x - pos.x;

        SetWalking(true);
        UpdateFlip(dx);

        if (Vector2.Distance(rb.position, new Vector2(currentPatrolTarget.x, rb.position.y)) <= arriveDistance)
        {
            if (waitRoutine == null)
                waitRoutine = StartCoroutine(WaitThenSwitchPoint());
        }
    }

    IEnumerator WaitThenSwitchPoint()
    {
        state = State.WaitAtPoint;
        SetWalking(true);

        Vector2 frozen = rb.position;
        float t = 0f;

        while (t < waitAtPatrolPoint && state == State.WaitAtPoint && !attackingLoop)
        {
            rb.MovePosition(frozen);
            t += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        if (state != State.WaitAtPoint) { waitRoutine = null; yield break; }

        currentPatrolTarget = (currentPatrolTarget == aPos) ? bPos : aPos;
        state = State.Patrol;

        waitRoutine = null;
    }

    void StopWaiting()
    {
        if (state == State.WaitAtPoint)
            state = State.Patrol;

        if (waitRoutine != null)
        {
            StopCoroutine(waitRoutine);
            waitRoutine = null;
        }
    }

    // ------------------ PERSEGUIR ------------------
    void ChaseMove()
    {
        if (player == null) { state = State.Patrol; return; }

        Vector2 pos = rb.position;
        Vector2 target = new Vector2(player.position.x, pos.y);

        Vector2 next = Vector2.MoveTowards(pos, target, moveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(next);

        float dx = target.x - pos.x;

        SetWalking(true);
        UpdateFlip(dx);
    }

    // ------------------ ATAQUE ------------------
    void StartAttack()
    {
        if (attackingLoop) return;
        StopWaiting();
        StartCoroutine(AttackLoop());
    }

    IEnumerator AttackLoop()
    {
        attackingLoop = true;
        state = State.Attack;

        // desactivar movimiento
        SetWalking(false);

        // (opcional) habilitar distancia
        if (ataqueDistancia != null) ataqueDistancia.enabled = true;

        while (true)
        {
            if (player == null) FindPlayer();
            if (player == null) break;

            bool canHit = CanSeePlayer(out float dist, attackRange);
            if (!canHit) break;

            // --- Ejecutar ataque ---
            SetWalking(false);

            // Disparar / pegar
            if (meleeAttack != null)
            {
                meleeAttack.DoAttack(); // ✅ aplica daño usando hitLayer del melee_universal
            }
            else if (ataqueDistancia != null)
            {
                ataqueDistancia.Disparar(player);
            }

            // Anim de ataque
            if (anim != null && !string.IsNullOrEmpty(attackTrigger))
            {
                anim.ResetTrigger(attackTrigger);
                anim.SetTrigger(attackTrigger);
            }

            // Espera anim + idle + cooldown
            yield return new WaitForSeconds(attackAnimDuration);
            yield return new WaitForSeconds(idleAfterAttack);
            yield return new WaitForSeconds(attackCooldown);
        }

        if (ataqueDistancia != null) ataqueDistancia.enabled = false;

        attackingLoop = false;
        state = State.Patrol;
        SetWalking(true);
    }

    // ------------------ VISIÓN / LOS ------------------
    bool CanSeePlayer(out float dist, float maxRange)
    {
        dist = Mathf.Infinity;
        if (player == null) return false;

        Vector2 origin = rb.position;
        Vector2 ppos = player.position;
        dist = Vector2.Distance(origin, ppos);

        if (dist > maxRange) return false;
        if (!IsInFrontCone(ppos)) return false;
        if (!HasLineOfSight(ppos, maxRange)) return false;

        return true;
    }

    bool IsInFrontCone(Vector2 targetPos)
    {
        Vector2 toTarget = (targetPos - rb.position).normalized;
        Vector2 forward = GetForward2D();
        return Vector2.Angle(forward, toTarget) <= viewAngle * 0.5f;
    }

    Vector2 GetForward2D()
    {
        Vector2 baseForward = spriteDefaultFacesRight ? Vector2.right : Vector2.left;
        if (sr != null && sr.flipX) baseForward *= -1f;
        return baseForward;
    }

    bool HasLineOfSight(Vector2 targetPos, float maxDistance)
    {
        Vector2 origin = rb.position;
        Vector2 dir = (targetPos - origin).normalized;
        return Physics2D.Raycast(origin, dir, maxDistance, obstacleLayer).collider == null;
    }

    // ------------------ ANIM / FLIP ------------------
    void SetWalking(bool walking)
    {
        if (isWalking == walking) return;
        isWalking = walking;
        if (anim != null) anim.SetBool(isWalkingBool, walking);
    }

    void UpdateFlip(float dx)
    {
        if (!useFlipX || sr == null) return;
        if (dx > 0.01f) sr.flipX = spriteDefaultFacesRight ? false : true;
        else if (dx < -0.01f) sr.flipX = spriteDefaultFacesRight ? true : false;
    }

    void FindPlayer()
    {
        GameObject p = GameObject.FindGameObjectWithTag(playerTag);
        if (p != null) player = p.transform;
    }

    // ------------------ GIZMOS ------------------
    void OnDrawGizmosSelected()
    {
        if (!drawGizmos) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (drawCone)
        {
            SpriteRenderer s = GetComponent<SpriteRenderer>();
            Vector2 forward = spriteDefaultFacesRight ? Vector2.right : Vector2.left;
            if (s != null && s.flipX) forward *= -1f;

            float half = viewAngle * 0.5f;
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + Rotate(forward, +half).normalized * detectionRange);
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + Rotate(forward, -half).normalized * detectionRange);
        }

        if (drawLineToPlayer)
        {
            GameObject p = GameObject.FindGameObjectWithTag(playerTag);
            if (p != null)
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawLine(transform.position, p.transform.position);
            }
        }
    }

    Vector2 Rotate(Vector2 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float ca = Mathf.Cos(rad);
        float sa = Mathf.Sin(rad);
        return new Vector2(ca * v.x - sa * v.y, sa * v.x + ca * v.y);
    }
}
