using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // Necesario si quieres reiniciar el nivel después

public class vida : MonoBehaviour
{
    [Header("Vida (Corazones)")]
    public int maxHealth = 3;
    private int currentHealth;
    [Header("Sonidos Vida")]
    public AudioSource audioSource; // Arrastra el AudioSource del jugador
    public AudioClip sonidoHerida;  // Arrastra el sonido de "daño"
    public AudioSource gameover;
    public AudioSource muerte;
    public AudioClip murio;
    [Header("Intentos (Game Over)")]
    public int intentosMaximos = 3;
    private int intentosActuales;
    public GameObject panelGameOver; // Arrastra aquí tu panel de Game Over

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
        intentosActuales = 0; // Empezamos con 0 muertes
        if (panelGameOver != null) panelGameOver.SetActive(false); // Escondemos el panel al empezar

        currentRespawnPoint = initialRespawnPoint != null ? initialRespawnPoint : transform;
        RefreshHeartsFull();
    }

    public void TakeDamage(int amount)
    {
        if (dead) return;
        if (audioSource != null && sonidoHerida != null)
        {
        audioSource.PlayOneShot(sonidoHerida);
        }
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

    // --- SISTEMA DE MUERTE Y RESPAWN MODIFICADO ---
    IEnumerator DieAndRespawn()
    {
        dead = true;
        intentosActuales++; // Sumamos una muerte
    if (muerte != null && murio != null)
    {
        muerte.PlayOneShot(murio);
    }
        // 1. Iniciar animación de muerte y detener físicas
        if (anim != null) anim.SetTrigger(deathTrigger);
        if (rb != null) { rb.linearVelocity = Vector2.zero; rb.simulated = false; }

        if (disableScriptsWhileDead != null)
            foreach (var s in disableScriptsWhileDead) if (s != null) s.enabled = false;

        // 2. Esperar a que se vea la animación de muerte
        yield return new WaitForSeconds(deathAnimDuration);

        // --- COMPROBACIÓN DE GAME OVER ---
        if (intentosActuales >= intentosMaximos)
        {
            MostrarGameOver();
            yield break; // Detenemos la corrutina aquí, no hay respawn
        }

        // 3. REINICIAR ENEMIGOS
        vida_enemigo[] todosLosEnemigos = Resources.FindObjectsOfTypeAll<vida_enemigo>();
        foreach (vida_enemigo en in todosLosEnemigos)
        {
            if (en.gameObject.scene.name != null) en.ReiniciarEnemigo();
        }

        // 4. Teletransporte al Checkpoint
        Transform rp = currentRespawnPoint != null ? currentRespawnPoint : initialRespawnPoint;
        if (rp != null) transform.position = rp.position;

        // 5. Restaurar Vida y UI
        currentHealth = maxHealth;
        yield return new WaitForEndOfFrame(); 
        RefreshHeartsFull(); 

        // 6. Resetear Animator
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

    void MostrarGameOver()
    {
        Debug.Log("GAME OVER: Has muerto " + intentosMaximos + " veces.");
        if (panelGameOver != null) 
        {
            panelGameOver.SetActive(true); //Mostramos el panel (el que configuramos antes)
            gameover.Play(); 
        }
        // Opcional: Congelar el tiempo de juego
        // Time.timeScale = 0f; 
    }

    // Para botones en el panel de Game Over

    // --- RESTO DE FUNCIONES (SetCheckpoint, Heal, etc) ---
    public void SetCheckpoint(Transform checkpoint) 
    {
        if (currentRespawnPoint != checkpoint)
        {
            currentRespawnPoint = checkpoint;
            HealOneHeart(); 
        }
    }

    public void HealOneHeart()
    {
        if (currentHealth < maxHealth && !dead)
        {
            currentHealth++;
            int idx = currentHealth - 1;
            if (idx >= 0 && idx < controlador_vida.Length && controlador_vida[idx] != null)
            {
                controlador_vida[idx].Rebind();
                controlador_vida[idx].Update(0f);
                controlador_vida[idx].Play(fullStateName, 0, 0f);
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
                heart.Rebind();
                heart.Update(0f);
                heart.Play(fullStateName, 0, 0f);
            }
        }
    }
    // Función para cerrar el juego
public void ReiniciarNivel()
{
    Time.timeScale = 1f;
    // Esta línea carga la escena de nuevo
    SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    gameover.Stop();
}

public void SalirDelJuego()
{
    gameover.Stop();
    #if UNITY_EDITOR
        // ESTA es la línea que te saca al editor. 
        // Solo debe estar AQUÍ, no en ReiniciarNivel.
        UnityEditor.EditorApplication.isPlaying = false; 
    #else
        Application.Quit();
    #endif
}
}