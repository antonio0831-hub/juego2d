using System.Collections;
using UnityEngine;

public class ataque_distancia : MonoBehaviour
{
    public enum ShotMode { AimAtTarget, StraightHorizontal }

    [Header("Modo de disparo")]
    public ShotMode shotMode = ShotMode.AimAtTarget;

    [Header("Cómo detectar hacia dónde mira el enemigo")]
    public bool useSpriteFlipX = true;      
    public bool defaultFacesRight = true;   

    [Header("Disparo")]
    public Transform firePoint;
    public GameObject boltPrefab;

    [Header("Bolt settings")]
    public float boltSpeed = 12f;
    public float boltMaxDistance = 6f;

    [Header("Quién dispara")]
    public bool fromPlayer = false;         

    [Header("Sincronización con animación")]
    public float fireDelay = 0.12f;         
    public bool blockWhileWaiting = true;   

    private bool waitingShot = false;

    public void Disparar(Transform target)
    {
        if (firePoint == null || boltPrefab == null) return;
        if (blockWhileWaiting && waitingShot) return;

        // ✅ MEJORA: Si el bicho no sabe quién es su target, lo buscamos aquí
        if (target == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) target = p.transform;
        }

        StartCoroutine(DelayedShot(target));
    }

    IEnumerator DelayedShot(Transform target)
    {
        waitingShot = true;

        if (fireDelay > 0f)
            yield return new WaitForSeconds(fireDelay);

        if (firePoint == null || boltPrefab == null)
        {
            waitingShot = false;
            yield break;
        }

        GameObject boltGO = Instantiate(boltPrefab, firePoint.position, Quaternion.identity);

        // ✅ Determinar dirección
        Vector2 dir;

        // Si tenemos un objetivo y queremos apuntar...
        if (shotMode == ShotMode.AimAtTarget && target != null)
        {
            dir = ((Vector2)target.position - (Vector2)firePoint.position).normalized;
        }
        else
        {
            // Si no hay objetivo, usamos hacia donde mira el sprite (GetFacingDir)
            dir = GetFacingDir();
        }

        // Inicializar bolt
        bolt b = boltGO.GetComponent<bolt>();
        if (b != null)
        {
            b.speed = boltSpeed;
            b.maxDistance = boltMaxDistance;
            b.SetOwnerAndTarget(fromPlayer);
            b.Init(dir);

            // ✅ EXTRA: Rotar el Bolt para que la punta mire al objetivo
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            boltGO.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            Rigidbody2D rb = boltGO.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.gravityScale = 0f;
                rb.linearVelocity = dir * boltSpeed;
            }
        }

        waitingShot = false;
    }

    Vector2 GetFacingDir()
    {
        float sign = 1f;
        var sr = GetComponent<SpriteRenderer>();
        
        // Si no hay SpriteRenderer en este objeto, lo buscamos en los hijos
        if (sr == null) sr = GetComponentInChildren<SpriteRenderer>();

        if (useSpriteFlipX && sr != null)
        {
            bool flip = sr.flipX;
            sign = (defaultFacesRight) ? (flip ? -1f : 1f) : (flip ? 1f : -1f);
        }
        else
        {
            sign = (transform.lossyScale.x >= 0) ? 1f : -1f;
        }

        return (sign >= 0) ? Vector2.right : Vector2.left;
    }
}