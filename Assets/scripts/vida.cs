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
    private static int vidasExtras = 3; 
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
    
    // 🔥 CLAVE: Guardamos la posición en variables estáticas para que no se borren al recargar escena
    private static Vector3 posicionCheckpointGuardada;
    private static bool hayCheckpointRegistrado = false;

    public Rigidbody2D rb;
    public MonoBehaviour[] scriptsDeMovimiento;
    public bool esInvencible = false;

    private bool dead = false;

    void Awake()
    {
        Time.timeScale = 1f;
    }

    void Start()
    {
        currentHealth = maxHealth;

        // ✅ Si tocamos un checkpoint antes, aparecemos ahí al empezar/recargar
        if (hayCheckpointRegistrado)
        {
            transform.position = posicionCheckpointGuardada;
        }
        else if (initialRespawnPoint != null)
        {
            transform.position = initialRespawnPoint.position;
        }

        if (panelGameOver != null) panelGameOver.SetActive(false);
        UpdateHeartsUI();
    }

    public void TakeDamage(int amount)
    {
        if (dead || esInvencible) return;
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
            hayCheckpointRegistrado = false; // Reset checkpoint al perder todo
            if (panelGameOver != null) 
            {
                panelGameOver.SetActive(true);
                if (gameoverAudio != null) gameoverAudio.Play();
                Time.timeScale = 0f;
            }
        }
        else
        {
            // Recarga la escena; el Start() se encargará de ponerte en el checkpoint
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void ToggleScripts(bool state)
    {
        foreach (var s in scriptsDeMovimiento) if (s != null) s.enabled = state;
    }

    // ✅ Guarda la posición del checkpoint de forma persistente
    public void SetCheckpoint(Transform cp) 
    {
        posicionCheckpointGuardada = cp.position;
        hayCheckpointRegistrado = true;
    }

    public void HealOneHeart()
    {
        if (!dead && currentHealth < maxHealth) 
        { 
            currentHealth++; 
            UpdateHeartsUI(); 
        }
    }

    // ✅ Forzamos al Animator a refrescarse para que se vea el cambio visual siempre
    void UpdateHeartsUI()
    {
        if (controlador_vida == null) return;

        for (int i = 0; i < controlador_vida.Length; i++)
        {
            if (controlador_vida[i] != null)
            {
                controlador_vida[i].Rebind(); // Reset del estado del animador
                string animName = (i < currentHealth) ? fullStateName : breakStateName;
                controlador_vida[i].Play(animName, 0, 0f);
            }
        }
    }

    public void ResetGameTotal()
    {
        vidasExtras = 3;
        hayCheckpointRegistrado = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SalirAlMenu(string Interfaz)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(Interfaz);
    }
}