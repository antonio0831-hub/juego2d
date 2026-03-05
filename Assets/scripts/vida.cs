using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class vida : MonoBehaviour
{
    [Header("Vida (Corazones)")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Sonidos")]
    public AudioSource audioSource;
    public AudioClip sonidoHerida;
    public AudioSource gameoverAudio;
    public AudioSource muerteAudio;
    public AudioClip murioClip;

    [Header("Sistema de 3 Oportunidades")]
    private static int vidasExtras = 3; // Tienes 3 vidas antes del Game Over definitivo
    public GameObject panelGameOver;

    [Header("UI Corazones")]
    public Animator[] controlador_vida;
    public string breakStateName = "corazon_vacio";
    public string fullStateName = "corazon_lleno";

    [Header("Muerte y Respawn")]
    public Animator anim;
    public string deathTrigger = "muerte";
    public float deathAnimDuration = 1.0f;
    public Transform initialRespawnPoint;
    private Transform currentRespawnPoint;
    public Rigidbody2D rb;
    public MonoBehaviour[] scriptsDeMovimiento;
    public bool esInvencible = false;

    private bool dead = false;

    void Awake()
    {
        Time.timeScale = 1f;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) => Time.timeScale = 1f;

    void Start()
    {
        currentHealth = maxHealth;
        currentRespawnPoint = initialRespawnPoint != null ? initialRespawnPoint : transform;
        if (panelGameOver != null) panelGameOver.SetActive(false);
        RefreshHeartsFull();
    }

public void TakeDamage(int amount)
{
    if (dead || esInvencible) return; // 🔥 Ahora comprueba si es invencible
    currentHealth = Mathf.Max(0, currentHealth - amount);
    UpdateHeartsUI();

    if (currentHealth <= 0) StartCoroutine(DieSequence());
}

    IEnumerator DieSequence()
    {
        dead = true;
        vidasExtras--;
        
        if (muerteAudio != null) muerteAudio.PlayOneShot(murioClip);
        if (anim != null) anim.SetTrigger(deathTrigger);
        if (rb != null) { rb.linearVelocity = Vector2.zero; rb.simulated = false; }
        
        ToggleScripts(false);

        yield return new WaitForSecondsRealtime(deathAnimDuration);

        if (vidasExtras <= 0)
        {
            if (panelGameOver != null) 
            {
                panelGameOver.SetActive(true);
                if (gameoverAudio != null) gameoverAudio.Play();
                Time.timeScale = 0f;
            }
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void ToggleScripts(bool state)
    {
        foreach (var s in scriptsDeMovimiento) if (s != null) s.enabled = state;
    }

    public void SetCheckpoint(Transform cp) => currentRespawnPoint = cp;

  public void HealOneHeart()
{
    // Solo curamos si el jugador no está muerto y le falta vida
    if (!dead && currentHealth < maxHealth) 
    { 
        currentHealth++; 
    }
    
    // Llamamos SIEMPRE a la actualización visual para asegurar que el Canvas refleje la vida real
    UpdateHeartsUI();
}

    void UpdateHeartsUI()
    {
        for (int i = 0; i < controlador_vida.Length; i++)
        {
            if (controlador_vida[i] != null)
                controlador_vida[i].Play(i < currentHealth ? fullStateName : breakStateName);
        }
    }

    void RefreshHeartsFull()
    {
        foreach (var h in controlador_vida) if (h != null) h.Play(fullStateName);
    }

    // --- LÍNEAS MODIFICADAS PARA QUE EL BOTÓN FUNCIONE BIEN ---

    public void ResetGameTotal()
    {
        vidasExtras = 3;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SalirAlMenu(string Interfaz)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(Interfaz);
    }

    public void ReiniciarNivelBoton()
    {
        vidasExtras = 3; // Corregido: Ahora usa 'vidasExtras' en lugar de 'muertesTotales'
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}