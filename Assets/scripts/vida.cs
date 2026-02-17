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

    IEnumerator DieAndRespawn()
    {
        dead = true;

        if (disableScriptsWhileDead != null)
            foreach (var s in disableScriptsWhileDead) if (s != null) s.enabled = false;

        if (rb != null) { rb.linearVelocity = Vector2.zero; rb.simulated = false; }

        if (anim != null) anim.SetTrigger(deathTrigger);

        yield return new WaitForSeconds(deathAnimDuration);

        Transform rp = currentRespawnPoint != null ? currentRespawnPoint : initialRespawnPoint;
        if (rp != null) transform.position = rp.position;

        // RESTAURAR VIDA
        currentHealth = maxHealth;
        
        // --- REFRESCAR UI ---
        yield return new WaitForEndOfFrame(); 
        RefreshHeartsFull(); 

        if (rb != null) rb.simulated = true;

        if (disableScriptsWhileDead != null)
            foreach (var s in disableScriptsWhileDead) if (s != null) s.enabled = true;

        dead = false;
    }

    public void SetCheckpoint(Transform checkpoint) => currentRespawnPoint = checkpoint;

    public void HealOneHeart()
    {
        if (currentHealth < maxHealth)
        {
            currentHealth++;
            int idx = currentHealth - 1;
            
            if (idx >= 0 && idx < controlador_vida.Length && controlador_vida[idx] != null)
            {
                // FORZAR REINICIO DEL ANIMATOR
                controlador_vida[idx].Rebind();
                controlador_vida[idx].Update(0f);
                controlador_vida[idx].Play(fullStateName, 0, 0f);
                Debug.Log("Corazón visual restaurado en índice: " + idx);
            }
        }
    }

    void RefreshHeartsFull()
    {
        if (controlador_vida == null) return;
        foreach (var heart in controlador_vida)
        {
            if (heart != null)
            {
                // FORZAR REINICIO DE TODOS LOS CORAZONES
                heart.Rebind();
                heart.Update(0f);
                heart.Play(fullStateName, 0, 0f);
            }
        }
    }
}