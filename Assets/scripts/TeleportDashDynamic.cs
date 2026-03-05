using UnityEngine;

public class TeleportDashDynamic : MonoBehaviour
{
    [Header("Punto dinámico (Empty hijo del player)")]
    public Transform dashPointChild;
    public float dashDistance = 2.0f;

    [Header("Input")]
    public KeyCode teleportKey = KeyCode.E;

    [Header("Animator")]
    public Animator animator;
    public string triggerName = "Teleport";

    [Header("Cooldown")]
    public float cooldown = 0.6f;

    [Header("Dirección 2D")]
    public SpriteRenderer spriteRenderer;

    [Header("Física y Colisión")]
    public Rigidbody2D rb2D;
    public LayerMask blockedLayers;
    public float overlapRadius = 0.18f;

    [Header("Invencibilidad")]
    public float duracionInvencibilidad = 0.3f; // Tiempo que serás inmune al daño
    private vida scriptVida;

    bool _canUse = true;

    void Start()
    {
        // Buscamos el script de vida en el mismo objeto
        scriptVida = GetComponent<vida>();
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
                DoTeleport(); 
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

    // 🎬 LLÁMALA DESDE EL ANIMATION EVENT O DIRECTAMENTE
    public void DoTeleport()
    {
        if (!_canUse || dashPointChild == null) return;

        Vector3 targetPos = dashPointChild.position;
        
        // Check colisión
        if (Physics2D.OverlapCircle(targetPos, overlapRadius, blockedLayers) != null) return;

        _canUse = false;

        // --- ACTIVAR INVENCIBILIDAD ---
        if (scriptVida != null)
        {
            scriptVida.esInvencible = true;
            Invoke(nameof(DesactivarInvencibilidad), duracionInvencibilidad);
        }

        // Realizar el Teleport
        if (rb2D != null)
        {
            rb2D.linearVelocity = Vector2.zero;
            rb2D.position = targetPos;
        }
        else
        {
            transform.position = targetPos;
        }

        Invoke(nameof(ResetCooldown), cooldown);
    }

    void DesactivarInvencibilidad()
    {
        if (scriptVida != null) scriptVida.esInvencible = false;
    }

    void ResetCooldown() => _canUse = true;
}