using System.Collections.Generic;
using UnityEngine;

public class melee_universal : MonoBehaviour
{
    public enum HitShape { Circle, Box }

    [Header("Forma del golpe")]
    public HitShape shape = HitShape.Circle;
    [Header("Audio")]
    public AudioSource fuenteDeAudio;
    public AudioClip sonidoAtaque;
    [Header("Input (solo si es Player)")]
    public bool useInput = false;              // Player: true / Enemigo: false
    public KeyCode attackKey = KeyCode.Mouse0;

    [Header("Hitbox")]
    public Transform attackPoint;
    public LayerMask hitLayer;
    public int damage = 1;
    public float cooldown;
    //public int cooljugador;

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
    //private bool atacado = false;

    void Start()
    {

    }
    void Update()
    {
        if (!useInput) return;
        if (Time.time < nextTime) return;

        if (Input.GetKeyDown(KeyCode.Mouse0) /*&& !atacado*/)
            {
            //atacado = true;
            anim.ResetTrigger(attackTrigger);
            anim.SetTrigger(attackTrigger);
            DoAttack();
           // StartCoroutine(reset());
            fuenteDeAudio.PlayOneShot(sonidoAtaque);
             }
    }

    // ✅ Llamar desde IA o desde input
    public void DoAttack()
    {

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
// Busca esta sección dentro del foreach de melee_universal.cs:
foreach (var h in hits)
{
    if (h == null) continue;

    // Evitar autohit
    if (h.transform == transform) continue;

    if (damageEnemies)
    {
        // 1. Objetos débiles del boss
        BossWeakObject weakObj =
            h.GetComponent<BossWeakObject>() ??
            h.GetComponentInParent<BossWeakObject>() ??
            h.transform.root.GetComponentInChildren<BossWeakObject>(true);

        if (weakObj != null && !damaged.Contains(weakObj))
        {
            damaged.Add(weakObj);

            if (debugHits)
                Debug.Log($"[melee_universal] Golpeando OBJETO DÉBIL: {weakObj.name}");

            weakObj.ReceiveHit();
            continue;
        }

        // 2. Vida del boss
        BossHealthController boss =
            h.GetComponent<BossHealthController>() ??
            h.GetComponentInParent<BossHealthController>() ??
            h.transform.root.GetComponentInChildren<BossHealthController>(true);

        if (boss != null && !damaged.Contains(boss))
        {
            damaged.Add(boss);

            if (debugHits)
                Debug.Log($"[melee_universal] Dañando BOSS: {boss.name}");

            boss.TakeDamage(damage);
            continue;
        }

        // 3. Enemigos normales
        vida_enemigo ve =
            h.GetComponent<vida_enemigo>() ??
            h.GetComponentInParent<vida_enemigo>() ??
            h.transform.root.GetComponentInChildren<vida_enemigo>(true);

        if (ve != null && !damaged.Contains(ve))
        {
            damaged.Add(ve);

            if (debugHits)
                Debug.Log($"[melee_universal] Dañando ENEMIGO: {ve.name} (fromPlayer={fromPlayer})");

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
    /*System.Collections.IEnumerator reset()
    {
        yield return new WaitForSeconds(cooldown);
        
        atacado = false;
        
    }*/
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
