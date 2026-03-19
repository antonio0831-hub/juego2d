using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossBehaviour : MonoBehaviour
{
    public enum BossState
    {
        Intro,
        Combat,
        Trapped,
        Dead
    }

    [Header("Estado")]
    public BossState currentState = BossState.Intro;

    [Header("Vida")]
    public int maxLives = 9;
    public int currentLives = 9;

    [Header("Puntos")]
    public Transform originalPoint;
    public Transform trapPoint;

    [Header("Referencias")]
    public Animator animator;
    public Rigidbody2D rb;
    public Collider2D bossCollider;

    [Header("Stun")]
    public float trappedDuration = 6f;
    public int maxDamagePerTrap = 3;

    [Header("Escena final")]
    public string finalSceneName = "FinalScene";
    public float deathDelay = 3f;

    [Header("Animator Parameters")]
    public string attack1Trigger = "Ataque1";
    public string attack2Trigger = "Ataque2";
    public string deathTrigger = "Muerte";

    [Header("Ataques")]
    public float timeBetweenAttacks = 2f;
    public int minAttack1BeforeAttack2 = 2;
    public int maxAttack1BeforeAttack2 = 3;

    private int damageTakenThisTrap = 0;
    private int brokenWeakObjects = 0;
    private int attack1Counter = 0;
    private int attack1TargetCount = 2;

    private bool isDead = false;
    private bool isTrapped = false;

    private Coroutine attackRoutine;
    private Coroutine trappedRoutine;

    private RigidbodyConstraints2D originalConstraints;
    private RigidbodyType2D originalBodyType;
    private bool originalIsTrigger;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (bossCollider == null)
            bossCollider = GetComponent<Collider2D>();

        if (rb != null)
        {
            originalConstraints = rb.constraints;
            originalBodyType = rb.bodyType;
        }

        if (bossCollider != null)
            originalIsTrigger = bossCollider.isTrigger;
    }

    private void Start()
    {
        currentLives = maxLives;
        ChooseNextAttack1Target();
    }

    public void StartBossFight()
    {
        if (isDead) return;

        currentState = BossState.Combat;
        isTrapped = false;
        brokenWeakObjects = 0;

        UnlockBossMovement();

        if (attackRoutine != null)
            StopCoroutine(attackRoutine);

        attackRoutine = StartCoroutine(CombatRoutine());

        Debug.Log("Boss en combate");
    }

    private IEnumerator CombatRoutine()
    {
        while (!isDead)
        {
            if (currentState == BossState.Combat)
            {
                if (attack1Counter < attack1TargetCount)
                {
                    DoAttack1();
                    attack1Counter++;
                }
                else
                {
                    DoAttack2();
                    attack1Counter = 0;
                    ChooseNextAttack1Target();
                }
            }

            yield return new WaitForSeconds(timeBetweenAttacks);
        }
    }

    private void DoAttack1()
    {
        if (animator != null && !string.IsNullOrEmpty(attack1Trigger))
            animator.SetTrigger(attack1Trigger);

        Debug.Log("Boss usa Ataque1");
    }

    private void DoAttack2()
    {
        if (animator != null && !string.IsNullOrEmpty(attack2Trigger))
            animator.SetTrigger(attack2Trigger);

        Debug.Log("Boss usa Ataque2");
    }

    private void ChooseNextAttack1Target()
    {
        attack1TargetCount = Random.Range(minAttack1BeforeAttack2, maxAttack1BeforeAttack2 + 1);
    }

    // Llama a este método cada vez que se rompe uno de los 2 objetos
    public void NotifyWeakObjectDestroyed()
    {
        if (isDead) return;
        if (currentState != BossState.Combat) return;

        brokenWeakObjects++;
        Debug.Log("Objeto débil roto. Total: " + brokenWeakObjects);

        if (brokenWeakObjects >= 2)
        {
            TriggerTrapState();
        }
    }

    public void TriggerTrapState()
    {
        if (isDead) return;
        if (isTrapped) return;
        if (currentState != BossState.Combat) return;

        if (trapPoint == null)
        {
            Debug.LogError("trapPoint no asignado");
            return;
        }

        currentState = BossState.Trapped;
        isTrapped = true;
        damageTakenThisTrap = 0;
        brokenWeakObjects = 0;

        transform.position = trapPoint.position;
        LockBossMovement();

        Debug.Log("Boss stuneado y teletransportado a trapPoint: " + trapPoint.position);

        if (trappedRoutine != null)
            StopCoroutine(trappedRoutine);

        trappedRoutine = StartCoroutine(TrappedRoutine());
    }

    private IEnumerator TrappedRoutine()
    {
        float timer = trappedDuration;

        while (timer > 0f)
        {
            if (isDead)
                yield break;

            timer -= Time.deltaTime;

            if (damageTakenThisTrap >= maxDamagePerTrap)
                break;

            yield return null;
        }

        ReturnToOriginalPoint();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;
        if (currentState != BossState.Trapped) return;
        if (amount <= 0) return;

        int remainingAllowedDamage = maxDamagePerTrap - damageTakenThisTrap;
        if (remainingAllowedDamage <= 0) return;

        int finalDamage = Mathf.Min(amount, remainingAllowedDamage);

        damageTakenThisTrap += finalDamage;
        currentLives -= finalDamage;

        Debug.Log("Boss recibe daño. Vida restante: " + currentLives);

        if (currentLives <= 0)
        {
            currentLives = 0;
            StartCoroutine(DeathRoutine());
        }
    }

    private void ReturnToOriginalPoint()
    {
        if (isDead) return;

        if (originalPoint == null)
        {
            Debug.LogError("originalPoint no asignado");
            return;
        }

        transform.position = originalPoint.position;

        UnlockBossMovement();

        isTrapped = false;
        currentState = BossState.Combat;

        Debug.Log("Boss volvió a originalPoint: " + originalPoint.position);
    }

    private IEnumerator DeathRoutine()
    {
        if (isDead)
            yield break;

        isDead = true;
        currentState = BossState.Dead;
        isTrapped = false;

        if (attackRoutine != null)
            StopCoroutine(attackRoutine);

        if (trappedRoutine != null)
            StopCoroutine(trappedRoutine);

        LockBossMovement();

        if (animator != null && !string.IsNullOrEmpty(deathTrigger))
            animator.SetTrigger(deathTrigger);

        Debug.Log("Boss muerto");

        yield return new WaitForSeconds(deathDelay);

        if (!string.IsNullOrEmpty(finalSceneName))
            SceneManager.LoadScene(finalSceneName);
        else
            Debug.LogError("No has asignado finalSceneName");
    }

    private void LockBossMovement()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.constraints = RigidbodyConstraints2D.FreezePositionX |
                             RigidbodyConstraints2D.FreezePositionY |
                             RigidbodyConstraints2D.FreezeRotation;
        }

        if (bossCollider != null)
            bossCollider.isTrigger = true;
    }
    public void TriggerStunnedState()
{
    TriggerTrapState();
}

    private void UnlockBossMovement()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = originalBodyType;
            rb.constraints = originalConstraints;
        }

        if (bossCollider != null)
            bossCollider.isTrigger = originalIsTrigger;
    }
}