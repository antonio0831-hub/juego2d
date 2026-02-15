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

    private int currentHealth;
    private Animator anim;
    private Rigidbody2D rb;
    private bool dead;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        currentHealth = Mathf.Max(1, maxHealth);
    }

    public void TakeDamage(int amount)
    {
        if (dead) return;

        amount = Mathf.Max(1, amount);
        currentHealth = Mathf.Max(0, currentHealth - amount);

        Debug.Log($"[vida_enemigo] {name} HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (dead) return;
        dead = true;

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
