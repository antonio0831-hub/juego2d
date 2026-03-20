using UnityEngine;

public class BossAttackController : MonoBehaviour
{
    [Header("Referencias")]
    public Animator animator;
    public Transform player;
    public Transform firePoint;
    public GameObject projectilePrefab;
    public BossController bossController;
    public BossBehaviour bossBehaviour;

    [Header("Ataque 1")]
    public float attackCooldown = 2f;

    [Header("Ataque 2")]
    public int minAttack1BeforeArea = 2;
    public int maxAttack1BeforeArea = 3;
    public int areaProjectileCount = 5;
    public float coneAngle = 45f;

    [Header("Daño proyectiles")]
    public int projectileDamage = 1;

    [Header("Triggers Animator")]
    public string attack1Trigger = "Ataque1";
    public string attack2Trigger = "Ataque2";

    [Header("Estado")]
    public bool canAttack = false;

    private int attack1Counter = 0;
    private int nextAreaAttackThreshold;
    private float cooldownTimer = 0f;
    private bool isAttacking = false;

    private void Start()
    {
        PickNextAreaThreshold();
        canAttack = false;
        isAttacking = false;
        cooldownTimer = attackCooldown;
    }

    private void Update()
    {
        if (bossController == null || !bossController.fightStarted || bossController.introPlaying)
        {
            canAttack = false;
            return;
        }

        if (bossBehaviour == null || bossBehaviour.currentState != BossBehaviour.BossState.Combat)
            return;

        if (!canAttack || isAttacking || player == null)
            return;

        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer <= 0f)
        {
            DecideNextAttack();
            cooldownTimer = attackCooldown;
        }
    }

    public void StartAttacking()
    {
        canAttack = true;
        isAttacking = false;
        cooldownTimer = attackCooldown;
        Debug.Log("BossAttackController: ataques activados");
    }

    public void StopAttacking()
    {
        canAttack = false;
        isAttacking = false;
        Debug.Log("BossAttackController: ataques detenidos");
    }

    private void DecideNextAttack()
    {
        if (isAttacking) return;

        isAttacking = true;

        if (attack1Counter >= nextAreaAttackThreshold)
        {
            attack1Counter = 0;
            PickNextAreaThreshold();

            Debug.Log("Trigger Ataque2");
            if (animator != null)
            {
                animator.ResetTrigger(attack1Trigger);
                animator.SetTrigger(attack2Trigger);
            }
        }
        else
        {
            attack1Counter++;

            Debug.Log("Trigger Ataque1");
            if (animator != null)
            {
                animator.ResetTrigger(attack2Trigger);
                animator.SetTrigger(attack1Trigger);
            }
        }
    }

    private void PickNextAreaThreshold()
    {
        nextAreaAttackThreshold = Random.Range(minAttack1BeforeArea, maxAttack1BeforeArea + 1);
    }

    public void FireAttack1()
    {
        if (player == null || firePoint == null || projectilePrefab == null) return;

        Vector2 dir = (player.position - firePoint.position).normalized;

        GameObject bullet = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        BossProjectile proj = bullet.GetComponent<BossProjectile>();
        if (proj != null)
        {
            proj.damage = projectileDamage;
            proj.SetDirection(dir);
        }

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    public void FireAttack2()
    {
        if (player == null || firePoint == null || projectilePrefab == null) return;

        Vector2 baseDir = (player.position - firePoint.position).normalized;
        float baseAngle = Mathf.Atan2(baseDir.y, baseDir.x) * Mathf.Rad2Deg;

        if (areaProjectileCount <= 1)
        {
            SpawnProjectileAtAngle(baseAngle);
            return;
        }

        float startAngle = baseAngle - coneAngle * 0.5f;
        float angleStep = coneAngle / (areaProjectileCount - 1);

        for (int i = 0; i < areaProjectileCount; i++)
        {
            float currentAngle = startAngle + angleStep * i;
            SpawnProjectileAtAngle(currentAngle);
        }
    }

    private void SpawnProjectileAtAngle(float angleDeg)
    {
        GameObject bullet = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        Vector2 dir = new Vector2(
            Mathf.Cos(angleDeg * Mathf.Deg2Rad),
            Mathf.Sin(angleDeg * Mathf.Deg2Rad)
        );

        BossProjectile proj = bullet.GetComponent<BossProjectile>();
        if (proj != null)
        {
            proj.damage = projectileDamage;
            proj.SetDirection(dir);
        }

        bullet.transform.rotation = Quaternion.Euler(0f, 0f, angleDeg);
    }

    public void EndAttack()
    {
        isAttacking = false;
    }
}