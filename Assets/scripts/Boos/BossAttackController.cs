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
    if (bossController == null) return;
    if (!bossController.fightStarted) return; // Asegúrate de que esto sea true
    if (!canAttack) return; // Esto debe ser true ahora
    
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
    isAttacking = false; // Desbloquea por si acaso se quedó trabado
    cooldownTimer = attackCooldown; // Reinicia el tiempo de espera
    Debug.Log("Sistema de ataque del Boss: ACTIVADO");
}

    public void StopAttacking()
    {
        canAttack = false;
        isAttacking = false;
    }

    void DecideNextAttack()
    {
        if (isAttacking) return;

        isAttacking = true;
        cooldownTimer = attackCooldown;

        if (attack1Counter >= nextAreaAttackThreshold)
        {
            attack1Counter = 0;
            PickNextAreaThreshold();

            Debug.Log("Trigger Attack2");
            if (animator != null)
                animator.SetTrigger("Attack2");
        }
        else
        {
            attack1Counter++;

            Debug.Log("Trigger Attack1");
            if (animator != null)
                animator.SetTrigger("Attack1");
        }
    }

    void PickNextAreaThreshold()
    {
        nextAreaAttackThreshold = Random.Range(minAttack1BeforeArea, maxAttack1BeforeArea + 1);
    }

    // Animation Event
    public void FireAttack1()
    {
        Debug.Log("FireAttack1 llamado");

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

    // Animation Event
    public void FireAttack2()
    {
        Debug.Log("FireAttack2 llamado");

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

    void SpawnProjectileAtAngle(float angleDeg)
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

    // Animation Event al final del clip de ataque
    public void EndAttack()
    {
        Debug.Log("EndAttack llamado");
        isAttacking = false;
    }
}