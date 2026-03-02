using UnityEngine;

public class TeleportDashDynamic_Event : MonoBehaviour
{
    [Header("Punto dinámico (Empty hijo del player)")]
    public Transform dashPointChild;     // Empty HIJO del player
    public float dashDistance = 2.0f;    // distancia constante delante/detrás

    [Header("Input")]
    public KeyCode teleportKey = KeyCode.E;

    [Header("Animator")]
    public Animator animator;
    public string triggerName = "Teleport";

    [Header("Cooldown")]
    public float cooldown = 0.6f;

    [Header("Dirección 2D")]
    public SpriteRenderer spriteRenderer; // usa flipX

    [Header("Opcional: Física 2D")]
    public Rigidbody2D rb2D;
    public bool zeroVelocityOnTeleport = true;

    [Header("Colisión destino (OverlapCircle)")]
    public Transform collisionCheckOrigin;      // si lo dejas null, usa el propio dashPointChild
    public float overlapRadius = 0.18f;
    public LayerMask blockedLayers;             // pon aquí Ground/Walls/Obstacles, NO el Player
    public bool drawGizmos = true;

    bool _canUse = true;

    void Reset()
    {
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        rb2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!_canUse) return;

        UpdateDashPoint();

        if (Input.GetKeyDown(teleportKey))
        {
            if (animator != null && !string.IsNullOrEmpty(triggerName))
                animator.SetTrigger(triggerName);
            else
                DoTeleport(); // fallback
        }
    }

    void UpdateDashPoint()
    {
        if (dashPointChild == null) return;

        bool facingLeft = (spriteRenderer != null && spriteRenderer.flipX);

        Vector3 local = dashPointChild.localPosition;
        local.x = (facingLeft ? -dashDistance : dashDistance);
        dashPointChild.localPosition = local;
    }

    // 🎬 LLÁMALA DESDE EL ANIMATION EVENT
    public void DoTeleport()
    {
        if (!_canUse) return;
        if (dashPointChild == null)
        {
            Debug.LogWarning("[TeleportDashDynamic_Event] Asigna dashPointChild (Empty hijo).");
            return;
        }

        Vector3 targetPos = dashPointChild.position;

        // ✅ Check colisión (OverlapCircle)
        Vector3 checkPos = (collisionCheckOrigin != null) ? collisionCheckOrigin.position : targetPos;

        // Importante: asegúrate de que blockedLayers NO incluya la capa del Player
        bool blocked = Physics2D.OverlapCircle(checkPos, overlapRadius, blockedLayers) != null;
        if (blocked)
        {
            // Si quieres, aquí podrías disparar un trigger de "FailTeleport"
            // animator?.SetTrigger("TeleportFail");
            return;
        }

        _canUse = false;

        if (rb2D != null)
        {
            if (zeroVelocityOnTeleport) rb2D.linearVelocity = Vector2.zero;
            rb2D.position = targetPos;
        }
        else
        {
            transform.position = targetPos;
        }

        Invoke(nameof(ResetCooldown), cooldown);
    }

    void ResetCooldown() => _canUse = true;

    void OnDrawGizmosSelected()
    {
        if (!drawGizmos) return;

        Transform t = collisionCheckOrigin != null ? collisionCheckOrigin : dashPointChild;
        if (t == null) return;

        Gizmos.DrawWireSphere(t.position, overlapRadius);
    }
}