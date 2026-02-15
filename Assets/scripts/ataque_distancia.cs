using System.Collections;
using UnityEngine;

public class ataque_distancia : MonoBehaviour
{
    public enum ShotMode { AimAtTarget, StraightHorizontal }

    [Header("Modo de disparo")]
    public ShotMode shotMode = ShotMode.AimAtTarget;

    [Header("Cómo detectar hacia dónde mira el enemigo")]
    public bool useSpriteFlipX = true;      // si tu enemigo gira con SpriteRenderer.flipX
    public bool defaultFacesRight = true;   // si tu sprite “por defecto” mira a la derecha

    [Header("Disparo")]
    public Transform firePoint;
    public GameObject boltPrefab;

    [Header("Bolt settings")]
    public float boltSpeed = 12f;
    public float boltMaxDistance = 6f;

    [Header("Quién dispara")]
    public bool fromPlayer = false;         // enemigo=false / player=true

    [Header("Sincronización con animación")]
    public float fireDelay = 0.12f;         // ⬅ ajusta esto hasta que coincida con el frame
    public bool blockWhileWaiting = true;   // ⬅ evita que spamee bolts si llaman varias veces

    private bool waitingShot = false;

    public void Disparar(Transform target)
    {
        if (firePoint == null || boltPrefab == null) return;

        // ✅ Anti-spam: si ya hay un disparo “en cola”, no crear otro
        if (blockWhileWaiting && waitingShot) return;

        StartCoroutine(DelayedShot(target));
    }

    IEnumerator DelayedShot(Transform target)
    {
        waitingShot = true;

        // Espera para sincronizar con la animación
        if (fireDelay > 0f)
            yield return new WaitForSeconds(fireDelay);

        if (firePoint == null || boltPrefab == null)
        {
            waitingShot = false;
            yield break;
        }

        GameObject boltGO = Instantiate(boltPrefab, firePoint.position, Quaternion.identity);

        // ✅ Elegir dirección según modo
        Vector2 dir = Vector2.right;

        if (shotMode == ShotMode.AimAtTarget)
        {
            if (target == null) dir = GetFacingDir();
            else dir = ((Vector2)target.position - (Vector2)firePoint.position).normalized;
        }
        else
        {
            dir = GetFacingDir(); // solo izq/der
        }

        // Inicializar bolt
        bolt b = boltGO.GetComponent<bolt>();
        if (b != null)
        {
            b.speed = boltSpeed;
            b.maxDistance = boltMaxDistance;
            b.SetOwnerAndTarget(fromPlayer);
            b.Init(dir);
        }
        else
        {
            // fallback por si no tiene script bolt
            Rigidbody2D rb = boltGO.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // importante: recto
                rb.gravityScale = 0f;
                rb.linearVelocity = dir * boltSpeed;
            }
        }

        waitingShot = false;
    }

    // Devuelve Vector2.right o Vector2.left según hacia dónde mira el enemigo
    Vector2 GetFacingDir()
    {
        float sign = 1f;

        if (useSpriteFlipX)
        {
            var sr = GetComponent<SpriteRenderer>();
            bool flip = (sr != null && sr.flipX);

            sign = (defaultFacesRight)
                ? (flip ? -1f : 1f)
                : (flip ? 1f : -1f);
        }
        else
        {
            sign = (transform.lossyScale.x >= 0) ? 1f : -1f;
        }

        return (sign >= 0) ? Vector2.right : Vector2.left;
    }
}
