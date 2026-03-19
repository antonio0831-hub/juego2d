using System.Collections;
using UnityEngine;

public class BossBehaviour : MonoBehaviour
{
    public enum BossState
    {
        Intro,
        Combat,
        Trapped,
        Returning,
        PhaseChange
    }

    [Header("Estado")]
    public BossState currentState = BossState.Intro;

    [Header("Vidas de fase")]
    public int maxLives = 9;
    public int currentLives = 9;

    [Header("Daño por ventana vulnerable")]
    public int maxDamagePerTrap = 3;
    private int damageTakenThisTrap = 0;

    [Header("Referencias")]
    public Animator animator;
    public Rigidbody2D rb;
    public Transform originalPoint;
    public Transform trapPoint;
    public BossWeakObjectManager weakObjectManager;

    [Header("Movimiento")]
    public float moveSpeed = 4f;

    [Header("Duración atrapado")]
    public float trappedDuration = 6f;

    [Header("Animaciones")]
    public string trappedTrigger = "Atrapado";
    public string returnTrigger = "Volver";
    public string phaseChangeTrigger = "CambioFase";

    private bool movingToTrap = false;
    private bool movingToOrigin = false;

    public void StartBossFight()
    {
        currentState = BossState.Combat;
        Debug.Log("Boss en estado COMBAT");
    }

    private void Update()
    {
        if (movingToTrap && trapPoint != null)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                trapPoint.position,
                moveSpeed * Time.deltaTime
            );

            if (Vector2.Distance(transform.position, trapPoint.position) < 0.05f)
            {
                transform.position = trapPoint.position;
                movingToTrap = false;
                StartCoroutine(TrappedRoutine());
            }
        }

        if (movingToOrigin && originalPoint != null)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                originalPoint.position,
                moveSpeed * Time.deltaTime
            );

            if (Vector2.Distance(transform.position, originalPoint.position) < 0.05f)
            {
                transform.position = originalPoint.position;
                movingToOrigin = false;
                currentState = BossState.Combat;

                if (weakObjectManager != null)
                    weakObjectManager.StartNewCycle();
            }
        }
    }

    public void TriggerTrapState()
    {
        Debug.Log("TriggerTrapState llamado. Estado actual: " + currentState);
        if (currentState != BossState.Combat)
        Debug.LogWarning("No se puede stunear porque el boss NO está en Combat");
            return;

        currentState = BossState.Trapped;
        damageTakenThisTrap = 0;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        movingToTrap = true;
        Debug.Log("Boss entrando en estado TRAPPED");
    }

    private IEnumerator TrappedRoutine()
    {
        if (animator != null)
            animator.SetTrigger(trappedTrigger);

        float timer = trappedDuration;

        while (timer > 0f)
        {
            timer -= Time.deltaTime;

            // Si ya recibió el máximo daño permitido en esta ventana, se acaba antes
            if (damageTakenThisTrap >= maxDamagePerTrap)
                break;

            yield return null;
        }

        ReturnToOriginalPosition();
    }

    public void TakeDamage(int amount)
    {
        if (currentState != BossState.Trapped)
            return;

        if (damageTakenThisTrap >= maxDamagePerTrap)
            return;

        int allowedDamage = maxDamagePerTrap - damageTakenThisTrap;
        int finalDamage = Mathf.Min(amount, allowedDamage);

        damageTakenThisTrap += finalDamage;
        currentLives -= finalDamage;

        Debug.Log("Boss recibe daño. Vidas restantes: " + currentLives);

        if (currentLives <= 0)
        {
            currentLives = 0;
            ChangePhase();
            return;
        }
    }

    private void ReturnToOriginalPosition()
    {
        if (animator != null)
            animator.SetTrigger(returnTrigger);

        currentState = BossState.Returning;
        movingToOrigin = true;
    }

    private void ChangePhase()
    {
        currentState = BossState.PhaseChange;

        if (animator != null)
            animator.SetTrigger(phaseChangeTrigger);

        Debug.Log("El boss cambia de fase.");
    }
}