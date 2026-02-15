using System.Collections;
using UnityEngine;

public class vida : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Corazones UI (Animators)")]
    public Animator[] controlador_vida;       // corazon_1, corazon_2, corazon_3
    public string breakStateName = "corazon_vacio";
    public string fullStateName = "corazon_lleno"; // si no tienes anim de lleno, lo dejamos opcional

    [Header("Muerte")]
    public Animator anim;                     // Animator del player
    public string deathTrigger = "muerte";
    public float deathAnimDuration = 0.7f;    // pon la duración real de tu clip muerte

    [Header("Respawn")]
    public Transform initialRespawnPoint;     // punto inicial
    private Transform currentRespawnPoint;    // checkpoint actual

    [Header("Desactivar al morir (opcional)")]
    public MonoBehaviour[] disableScriptsWhileDead; // movimiento, ataque, etc.
    public Rigidbody2D rb;                    // opcional (auto)

    private bool dead;

    void Awake()
    {
        if (anim == null) anim = GetComponent<Animator>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        currentHealth = maxHealth;

        // Respawn inicial
        currentRespawnPoint = initialRespawnPoint != null ? initialRespawnPoint : transform;

        // UI inicial (si quieres reiniciar corazones al empezar)
        RefreshHeartsFull();
    }

    public void TakeDamage(int amount)
    {
        if (dead) return;

        amount = Mathf.Max(1, amount);
        int prev = currentHealth;

        currentHealth = Mathf.Max(0, currentHealth - amount);

        // Animar corazones que se pierden
        for (int hp = prev; hp > currentHealth; hp--)
        {
            int idx = hp - 1; // 3->2 rompe index 2
            if (idx >= 0 && idx < controlador_vida.Length && controlador_vida[idx] != null)
            {
                controlador_vida[idx].Play(breakStateName, 0, 0f);
            }
        }

        if (currentHealth <= 0)
        {
            StartCoroutine(DieAndRespawn());
        }
    }

    IEnumerator DieAndRespawn()
    {
        dead = true;

        // Desactivar scripts (movimiento/ataque)
        if (disableScriptsWhileDead != null)
        {
            foreach (var s in disableScriptsWhileDead)
                if (s != null) s.enabled = false;
        }

        // Parar física
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false;
        }

        // Animación muerte
        if (anim != null && !string.IsNullOrEmpty(deathTrigger))
        {
            anim.ResetTrigger(deathTrigger);
            anim.SetTrigger(deathTrigger);
        }

        // Esperar a que se vea la muerte
        yield return new WaitForSeconds(deathAnimDuration);

        // Respawn
        Transform rp = currentRespawnPoint != null ? currentRespawnPoint : initialRespawnPoint;
        if (rp != null)
        {
            transform.position = rp.position;
        }

        // Reactivar física
        if (rb != null)
        {
            rb.simulated = true;
            rb.linearVelocity = Vector2.zero;
        }

        // Restaurar vida
        currentHealth = maxHealth;
        RefreshHeartsFull();

        // Reactivar scripts
        if (disableScriptsWhileDead != null)
        {
            foreach (var s in disableScriptsWhileDead)
                if (s != null) s.enabled = true;
        }

        dead = false;
    }

    // ✅ lo llama el checkpoint
    public void SetCheckpoint(Transform checkpoint)
    {
        currentRespawnPoint = checkpoint;
        Debug.Log("[vida] Checkpoint actualizado: " + checkpoint.name);
    }

    // --- UI Helpers ---
    void RefreshHeartsFull()
    {
        if (controlador_vida == null) return;

        for (int i = 0; i < controlador_vida.Length; i++)
        {
            if (controlador_vida[i] == null) continue;

            // Si tienes un estado/clip de corazón lleno, úsalo
            if (!string.IsNullOrEmpty(fullStateName))
                controlador_vida[i].Play(fullStateName, 0, 0f);
            else
                controlador_vida[i].Rebind(); // resetea animator al default
        }
    }

    public bool IsDead() => dead;
    public int GetHealth() => currentHealth;
}
