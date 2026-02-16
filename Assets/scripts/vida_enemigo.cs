using UnityEngine;

[DisallowMultipleComponent]
public class vida_enemigo : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 1;

    [Header("Muerte")]
    public string deathTrigger = "muerte";
    public float destroyAfterSeconds = 1f;

    [Header("Desactivar al morir")]
    public MonoBehaviour[] disableScriptsOnDeath;   // IA, ataque, etc.
    public Collider2D[] disableCollidersOnDeath;    // Colliders del enemigo

    [Header("Kills del player (carga)")]
    public bool countKillOnlyIfFromPlayer = true;   // ✅ lo que pediste
    public int killValue = 1;                       // por si algún enemigo cuenta +1

    private int currentHealth;
    private Animator anim;
    private Rigidbody2D rb;
    private bool dead;

    // ✅ Guarda quién dio el último golpe
    private bool lastHitFromPlayer = false;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        currentHealth = Mathf.Max(1, maxHealth);
    }

    // --- Daño “simple” (compatibilidad con scripts viejos) ---
    // Si llamas a esto, asumimos que NO viene del player (para evitar sumar sin querer).
    public void TakeDamage(int amount)
    {
        TakeDamage(amount, false);
    }

    // ✅ Daño con “origen”: desde player o desde enemigo
    public void TakeDamage(int amount, bool fromPlayer)
    {
        if (dead) return;

        amount = Mathf.Max(1, amount);
        currentHealth = Mathf.Max(0, currentHealth - amount);

        lastHitFromPlayer = fromPlayer;

        Debug.Log($"[vida_enemigo] {name} HP: {currentHealth} (fromPlayer={fromPlayer})");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (dead) return;
        dead = true;

        // ✅ Si quieres que SOLO cuente cuando mata el player:
        if (!countKillOnlyIfFromPlayer || lastHitFromPlayer)
    {
    GameObject p = GameObject.FindGameObjectWithTag("Player");
    if (p != null)
    {
        var charge = p.GetComponent<KillChargeManager>();
        if (charge != null)
        {
            charge.AddKill(Mathf.Max(1, killValue)); // ✅ aquí
        }
        else
        {
            Debug.LogWarning("[vida_enemigo] No existe KillChargeManager en el Player.");
        }
    }
    }


        // ✅ Animación muerte
        if (anim != null && !string.IsNullOrEmpty(deathTrigger))
        {
            anim.ResetTrigger(deathTrigger);
            anim.SetTrigger(deathTrigger);
        }

        // ✅ Parar físicas
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        // ✅ Desactivar scripts (IA, ataque...)
        if (disableScriptsOnDeath != null)
        {
            foreach (var s in disableScriptsOnDeath)
                if (s != null) s.enabled = false;
        }

        // ✅ Desactivar colliders
        if (disableCollidersOnDeath != null)
        {
            foreach (var c in disableCollidersOnDeath)
                if (c != null) c.enabled = false;
        }

        // ✅ Destruir tras la animación
        Destroy(gameObject, Mathf.Max(0.01f, destroyAfterSeconds));
    }

    public int GetHealth() => currentHealth;
    public bool IsDead() => dead;
}
