using UnityEngine;

public class JugadorAtaqueDistancia : MonoBehaviour
{
    [Header("Referencias")]
    public Animator animator;
    public Transform firePoint;
    public GameObject boltPrefab;
    public AudioSource audioSource;
    public AudioClip sonidoDisparo;

    [Header("Ajustes del bolt")]
    public float boltSpeed = 12f;
    public float boltMaxDistance = 8f;
    public int boltDamage = 1;

    [Header("Input")]
    public KeyCode teclaDisparo = KeyCode.F;

    [Header("Dirección")]
    public SpriteRenderer spriteRenderer;
    public bool defaultFacesRight = true;

    [Header("Control")]
    public float cooldown = 0.4f;

    private float lastShotTime;

    void Update()
    {
        if (!PowerUpResistent.tieneAtaqueDistancia)
            return;

        if (Input.GetKeyDown(teclaDisparo) && Time.time >= lastShotTime + cooldown)
        {
            lastShotTime = Time.time;

            if (animator != null)
                animator.SetTrigger("ataque_distancia");
        }
    }

    // ESTE método se llama desde un Animation Event
    public void SpawnBolt()
    {
        if (!PowerUpResistent.tieneAtaqueDistancia) return;
        if (firePoint == null || boltPrefab == null) return;

        GameObject boltGO = Instantiate(boltPrefab, firePoint.position, Quaternion.identity);

        Vector2 dir = GetFacingDirection();

        bolt boltScript = boltGO.GetComponent<bolt>();
        if (boltScript != null)
        {
            boltScript.speed = boltSpeed;
            boltScript.maxDistance = boltMaxDistance;
            boltScript.damage = boltDamage;
            boltScript.SetOwnerAndTarget(true); // true = disparado por el player
            boltScript.Init(dir);
        }

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        boltGO.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        if (audioSource != null && sonidoDisparo != null)
            audioSource.PlayOneShot(sonidoDisparo);
    }

    private Vector2 GetFacingDirection()
    {
        float sign = 1f;

        if (spriteRenderer != null)
        {
            bool flip = spriteRenderer.flipX;
            sign = defaultFacesRight ? (flip ? -1f : 1f) : (flip ? 1f : -1f);
        }
        else
        {
            sign = transform.lossyScale.x >= 0 ? 1f : -1f;
        }

        return sign >= 0 ? Vector2.right : Vector2.left;
    }
}