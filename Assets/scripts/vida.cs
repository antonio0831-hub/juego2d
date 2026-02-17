using System.Collections;
using UnityEngine;

public class vida : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Corazones UI (Animators)")]
    public Animator[] controlador_vida; 
    public string breakStateName = "corazon_vacio";
    public string fullStateName = "corazon_lleno"; 

    [Header("Muerte")]
    public Animator anim;
    public string deathTrigger = "muerte";
    public float deathAnimDuration = 1.0f;

    [Header("Respawn")]
    public Transform initialRespawnPoint;      
    private Transform currentRespawnPoint;    

    [Header("Componentes a desactivar")]
    public MonoBehaviour[] disableScriptsWhileDead; 
    public Rigidbody2D rb;                    

    private bool dead;

    void Awake()
    {
        if (anim == null) anim = GetComponent<Animator>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        currentHealth = maxHealth;
        currentRespawnPoint = initialRespawnPoint != null ? initialRespawnPoint : transform;
        RefreshHeartsFull();
    }

    // --- SISTEMA DE DAÑO ---
    public void TakeDamage(int amount)
    {
        if (dead) return;

        int prev = currentHealth;
        currentHealth = Mathf.Max(0, currentHealth - amount);

        for (int hp = prev; hp > currentHealth; hp--)
        {
            int idx = hp - 1; 
            if (idx >= 0 && idx < controlador_vida.Length && controlador_vida[idx] != null)
            {
                controlador_vida[idx].Play(breakStateName, 0, 0f);
            }
        }

        if (currentHealth <= 0) StartCoroutine(DieAndRespawn());
    }

    // --- SISTEMA DE CURACIÓN (CHECKPOINT) ---
    public void SetCheckpoint(Transform checkpoint) 
    {
        // Si es un checkpoint nuevo, actualizamos y curamos
        if (currentRespawnPoint != checkpoint)
        {
            currentRespawnPoint = checkpoint;
            HealOneHeart(); 
        }
    }

    public void HealOneHeart()
    {
        // Solo cura si le falta vida y no está muerto
        if (currentHealth < maxHealth && !dead)
        {
            currentHealth++;
            int idx = currentHealth - 1;
            
            if (idx >= 0 && idx < controlador_vida.Length && controlador_vida[idx] != null)
            {
                // Forzar al corazón a mostrarse lleno visualmente
                controlador_vida[idx].Rebind();
                controlador_vida[idx].Update(0f);
                controlador_vida[idx].Play(fullStateName, 0, 0f);
                Debug.Log("Corazón recuperado en checkpoint. Vida actual: " + currentHealth);
            }
        }
    }

    // --- SISTEMA DE MUERTE Y RESPAWN ---
    IEnumerator DieAndRespawn()
    {
        dead = true;

        // 1. Iniciar animación de muerte y detener físicas
        if (anim != null) anim.SetTrigger(deathTrigger);
        if (rb != null) { rb.linearVelocity = Vector2.zero; rb.simulated = false; }

        if (disableScriptsWhileDead != null)
            foreach (var s in disableScriptsWhileDead) if (s != null) s.enabled = false;

        // 2. Esperar a que se vea la animación de muerte
        yield return new WaitForSeconds(deathAnimDuration);

        // 3. REINICIAR ENEMIGOS (sin recargar escena para mantener checkpoints)
        vida_enemigo[] todosLosEnemigos = Resources.FindObjectsOfTypeAll<vida_enemigo>();
        foreach (vida_enemigo en in todosLosEnemigos)
        {
            // Solo resetear si es un objeto de la escena (no un prefab)
            if (en.gameObject.scene.name != null) en.ReiniciarEnemigo();
        }

        // 4. Teletransporte al Checkpoint
        Transform rp = currentRespawnPoint != null ? currentRespawnPoint : initialRespawnPoint;
        if (rp != null) transform.position = rp.position;

        // 5. Restaurar Vida y UI
        currentHealth = maxHealth;
        yield return new WaitForEndOfFrame(); 
        RefreshHeartsFull(); 

        // 6. Resetear Animator del jugador para que pueda volver a caminar
        if (anim != null)
        {
            anim.Rebind();
            anim.Update(0f);
        }

        // 7. Reactivar todo
        if (rb != null) rb.simulated = true;
        if (disableScriptsWhileDead != null)
            foreach (var s in disableScriptsWhileDead) if (s != null) s.enabled = true;

        dead = false;
    }

    void RefreshHeartsFull()
    {
        if (controlador_vida == null) return;
        foreach (var heart in controlador_vida)
        {
            if (heart != null)
            {
                heart.Rebind();
                heart.Update(0f);
                heart.Play(fullStateName, 0, 0f);
            }
        }
    }
}