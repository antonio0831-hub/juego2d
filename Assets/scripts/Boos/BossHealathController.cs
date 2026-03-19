using System.Collections;
using UnityEngine;

public class BossHealthController : MonoBehaviour
{
    public enum BossState
    {
        Intro,
        Combat,
        Stunned,
        Returning,
        Dead
    }

    [Header("Vida")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Estado")]
    public BossState currentState = BossState.Intro;

    [Header("Referencias")]
    public Animator animator;
    public Rigidbody2D rb;

    [Header("Posición original")]
    public Transform originalPoint;
    public float returnSpeed = 4f;

    [Header("Stun / atrapado")]
    public float stunnedDuration = 5f;
    public string stunTriggerName = "Stunned";
    public string returnTriggerName = "Return";
    public bool canTakeExtraDamageWhenStunned = true;

    private bool fightStarted = false;
    private bool isInvulnerable = false;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (currentState == BossState.Returning && originalPoint != null)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                originalPoint.position,
                returnSpeed * Time.deltaTime
            );

            if (Vector2.Distance(transform.position, originalPoint.position) < 0.05f)
            {
                transform.position = originalPoint.position;
                currentState = BossState.Combat;
            }
        }
    }

    public void StartBossFight()
    {
        fightStarted = true;
        currentState = BossState.Combat;
    }

    public void TakeDamage(int damage)
    {
        if (currentState == BossState.Dead) return;
        if (isInvulnerable) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log("Boss vida actual: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void TriggerStunnedState()
    {
        if (currentState == BossState.Dead) return;
        if (currentState == BossState.Stunned) return;
        if (currentState == BossState.Returning) return;

        StartCoroutine(StunnedRoutine());
    }

    private IEnumerator StunnedRoutine()
    {
        currentState = BossState.Stunned;

        // parar movimiento
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezePositionX |
                             RigidbodyConstraints2D.FreezePositionY |
                             RigidbodyConstraints2D.FreezeRotation;
        }

        if (animator != null && !string.IsNullOrEmpty(stunTriggerName))
        {
            animator.SetTrigger(stunTriggerName);
        }

        yield return new WaitForSeconds(stunnedDuration);

        // quitar bloqueo físico
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        currentState = BossState.Returning;

        if (animator != null && !string.IsNullOrEmpty(returnTriggerName))
        {
            animator.SetTrigger(returnTriggerName);
        }
    }

    private void Die()
    {
        currentState = BossState.Dead;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        Debug.Log("Boss derrotado");
    }
}