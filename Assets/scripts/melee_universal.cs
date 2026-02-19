using System.Collections.Generic;
using UnityEngine;

public class melee_universal : MonoBehaviour
{
    public enum HitShape { Circle, Box }

    [Header("Forma del golpe")]
    public HitShape shape = HitShape.Circle;

    [Header("Input (solo si es Player)")]
    public bool useInput = false;              // Player: true / Enemigo: false
    public KeyCode attackKey = KeyCode.Mouse0;

    [Header("Hitbox")]
    public Transform attackPoint;
    public LayerMask hitLayer;
    public int damage = 1;
    public float cooldown = 0.35f;

    [Header("Círculo")]
    public float radius = 0.6f;

    [Header("Caja (solo delante)")]
    public Vector2 boxSize = new Vector2(1.2f, 0.8f);

    [Header("A quién daña")]
    public bool damagePlayer = false;
    public bool damageEnemies = true;

    [Header("Origen del daño (IMPORTANTE)")]
    public bool fromPlayer = true; // ✅ en el melee del PLAYER: true / melee de ENEMIGO: false

    [Header("Animator (opcional)")]
    public Animator anim;
    public string attackTrigger = "ataque";

    [Header("Debug")]
    public bool debugHits = false;

    private float nextTime;
    private float poderatacar;

    void start()
    {
        poderatacar = true;
    }
    void Update()
    {
        if (!useInput) return;
        if (Time.time < nextTime) return;

        if (Input.GetKeyDown(attackKey) && poderatacar==true)
            DoAttack();
    }

    // ✅ Llamar desde IA o desde input
    public void DoAttack()
    {
        puedoatacar = false;
        StartCoroutine(cool());
        if (Time.time < nextTime) return;
        nextTime = Time.time + cooldown;

        if (anim != null && !string.IsNullOrEmpty(attackTrigger))
        {
            anim.ResetTrigger(attackTrigger);
            anim.SetTrigger(attackTrigger);
        }

        if (attackPoint == null)
        {
            Debug.LogWarning("[melee_universal] Falta attackPoint.");
            return;
        }

        Collider2D[] hits =
            (shape == HitShape.Circle)
            ? Physics2D.OverlapCircleAll(attackPoint.position, radius, hitLayer)
            : Physics2D.OverlapBoxAll(attackPoint.position, boxSize, 0f, hitLayer);

        if (debugHits)
            Debug.Log($"[melee_universal] Hits detectados: {hits.Length}");

        // ✅ evita pegar varias veces al mismo objetivo si tiene varios colliders
        HashSet<object> damaged = new HashSet<object>();

        foreach (var h in hits)
        {
            if (h == null) continue;

            // Evitar autohit
            if (h.transform == transform) continue;

            if (damageEnemies)
            {
                vida_enemigo ve =
                    h.GetComponent<vida_enemigo>() ??
                    h.GetComponentInParent<vida_enemigo>() ??
                    h.transform.root.GetComponentInChildren<vida_enemigo>(true);

                if (ve != null && !damaged.Contains(ve))
                {
                    damaged.Add(ve);

                    if (debugHits)
                        Debug.Log($"[melee_universal] Dañando ENEMIGO: {ve.name} (fromPlayer={fromPlayer})");

                    // ✅ CLAVE: pasar fromPlayer para que cuente kill solo si mata el player
                    ve.TakeDamage(damage, fromPlayer);
                    continue;
                }
            }

            if (damagePlayer)
            {
                vida vp =
                    h.GetComponent<vida>() ??
                    h.GetComponentInParent<vida>() ??
                    h.transform.root.GetComponentInChildren<vida>(true);

                if (vp != null && !damaged.Contains(vp))
                {
                    damaged.Add(vp);

                    if (debugHits)
                        Debug.Log($"[melee_universal] Dañando PLAYER: {vp.name}");

                    vp.TakeDamage(damage);
                    continue;
                }
            }

            if (debugHits)
                Debug.Log($"[melee_universal] Golpeó '{h.name}' pero no encontró objetivo válido.");
        }
    }
    IEnumerator cool()
    {
        yield return new WaitForSeconds(cooldown);
        puedoatacar=true
    }
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.yellow;

        if (shape == HitShape.Circle)
            Gizmos.DrawWireSphere(attackPoint.position, radius);
        else
            Gizmos.DrawWireCube(attackPoint.position, boxSize);
    }
}
