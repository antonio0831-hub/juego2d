using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BossBehaviour : MonoBehaviour
{
    public enum BossState
    {
        Intro,
        Combat,
        Dead
    }

    [Header("Estado")]
    public BossState currentState = BossState.Intro;

    [Header("Vida del boss")]
    public int maxLives = 10;
    public int currentLives = 10;

    [Header("UI")]
    public Slider healthSlider;

    [Header("Referencias")]
    public Animator animator;
    public BossAttackController attackController;
    public BossWeakObjectManager weakObjectManager;

    [Header("Muerte")]
    public string deathTrigger = "Muerte";
    public float deathDelay = 3f;
    public string finalSceneName = "FinalScene";

    private bool isDead = false;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (attackController == null)
            attackController = GetComponent<BossAttackController>();

        if (weakObjectManager == null)
            weakObjectManager = Object.FindFirstObjectByType<BossWeakObjectManager>();
    }

    private void Start()
    {

    currentLives = maxLives;
    if (healthSlider != null) healthSlider.maxValue = maxLives;

        if (weakObjectManager != null)
        currentState = BossState.Intro;
    }

    public void StartBossFight()
    {
        if (isDead) return;

        currentState = BossState.Combat;

        if (healthSlider != null)
            healthSlider.gameObject.SetActive(true);

        if (weakObjectManager != null)
            weakObjectManager.StartNewCycle();

        if (attackController != null)
            attackController.StartAttacking();

        Debug.Log("Boss en combate");
    }

public void TakeBossDamage(int amount)
{
    if (isDead) 
    {
        Debug.Log("Dañando al Boss: Ignorado porque ya está muerto.");
        return;
    }
    
    if (currentState != BossState.Combat) 
    {
        Debug.LogWarning($"Dañando al Boss: Ignorado porque el estado actual es {currentState} (debe ser Combat).");
        return;
    }

    currentLives -= amount;
    if (currentLives < 0) currentLives = 0;

    if (healthSlider != null) healthSlider.value = currentLives;

    Debug.Log($"<color=magenta>BOSS DAÑADO:</color> Recibido: {amount} | Vida restante: {currentLives}");

    if (currentLives <= 0) StartCoroutine(DeathRoutine());
}

    private IEnumerator DeathRoutine()
    {
        if (isDead) yield break;

        isDead = true;
        currentState = BossState.Dead;

        if (attackController != null)
            attackController.StopAttacking();

        if (weakObjectManager != null)
            weakObjectManager.HideAllObjects();

        if (animator != null && !string.IsNullOrEmpty(deathTrigger))
            animator.SetTrigger(deathTrigger);

        Debug.Log("Boss muerto");

        yield return new WaitForSeconds(deathDelay);

        if (!string.IsNullOrEmpty(finalSceneName))
            SceneManager.LoadScene(finalSceneName);
        else
            Debug.LogError("No has asignado finalSceneName");
    }
}