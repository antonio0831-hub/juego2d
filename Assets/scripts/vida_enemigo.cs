using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class vida_enemigo : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 1;
    private int currentHealth;

    [Header("Muerte")]
    public string deathTrigger = "muerte";
    public float deactivateAfterSeconds = 1f;

    [Header("Desactivar al morir")]
    public MonoBehaviour[] disableScriptsOnDeath;   // IA, ataque, etc.
    public Collider2D[] disableCollidersOnDeath;    // Colliders del enemigo

    [Header("Kills del player (carga)")]
    public bool countKillOnlyIfFromPlayer = true;
    public int killValue = 1;

    // Variables privadas para el sistema de Reset
    private Animator anim;
    private Rigidbody2D rb;
    private bool dead;
    private bool lastHitFromPlayer = false;
    
    // Guardado de estado inicial
    private Vector3 posicionInicial;
    private Quaternion rotacionInicial;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // Guardamos la posición y rotación donde colocaste al enemigo en el editor
        posicionInicial = transform.position;
        rotacionInicial = transform.rotation;

        currentHealth = Mathf.Max(1, maxHealth);
    }

    // Daño sin especificar origen (asume falso)
    public void TakeDamage(int amount)
    {
        TakeDamage(amount, false);
    }

    // Daño con origen
    public void TakeDamage(int amount, bool fromPlayer)
    {
        if (dead) return;

        currentHealth = Mathf.Max(0, currentHealth - amount);
        lastHitFromPlayer = fromPlayer;

        Debug.Log($"[vida_enemigo] {name} HP: {currentHealth} (desde Player: {fromPlayer})");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (dead) return;
        dead = true;

        // Lógica de KillChargeManager (Suma puntos al jugador)
        if (!countKillOnlyIfFromPlayer || lastHitFromPlayer)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
            {
                var charge = p.GetComponent<KillChargeManager>();
                if (charge != null) charge.AddKill(Mathf.Max(1, killValue));
            }
        }

        // Animación de muerte
        if (anim != null && !string.IsNullOrEmpty(deathTrigger))
        {
            anim.SetTrigger(deathTrigger);
        }

        // Desactivar físicas y componentes
        if (rb != null) rb.simulated = false;

        foreach (var s in disableScriptsOnDeath) if (s != null) s.enabled = false;
        foreach (var c in disableCollidersOnDeath) if (c != null) c.enabled = false;

        // IMPORTANTE: En lugar de Destroy, usamos una Corrutina para desactivar
        StartCoroutine(DesactivarEnemigo());
    }

    IEnumerator DesactivarEnemigo()
    {
        yield return new WaitForSeconds(Mathf.Max(0.01f, deactivateAfterSeconds));
        gameObject.SetActive(false); // El enemigo "desaparece" pero sigue en la escena
    }

    // --- ESTA FUNCIÓN LA LLAMA EL JUGADOR AL MORIR ---
    public void ReiniciarEnemigo()
    {
        // Reset de estado
        dead = false;
        currentHealth = maxHealth;
        lastHitFromPlayer = false;

        // Volver a posición original
        transform.position = posicionInicial;
        transform.rotation = rotacionInicial;
        
        // Activar el objeto
        gameObject.SetActive(true);

        // Reactivar físicas y componentes
        if (rb != null)
        {
            rb.simulated = true;
            rb.linearVelocity = Vector2.zero; // Resetea movimiento si estaba cayendo
            rb.angularVelocity = 0f;
        }

        foreach (var s in disableScriptsOnDeath) if (s != null) s.enabled = true;
        foreach (var c in disableCollidersOnDeath) if (c != null) c.enabled = true;

        // Resetear Animator (para que no se quede en la animación de muerte)
        if (anim != null)
        {
            anim.Rebind();
            anim.Update(0f);
        }
    }

    public int GetHealth() => currentHealth;
    public bool IsDead() => dead;
}