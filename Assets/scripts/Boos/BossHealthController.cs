using UnityEngine;
using UnityEngine.UI;

public class BossHealthController : MonoBehaviour
{
    [Header("Vida UI")]
    public int maxHealth = 100;
    public int currentHealth;
    public Slider healthSlider;

    [Header("Referencias")]
    public Rigidbody2D rb;
    public BossBehaviour bossBehaviour;

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (bossBehaviour == null)
            bossBehaviour = GetComponent<BossBehaviour>();
    }

    private void Start()
    {
        currentHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
            healthSlider.gameObject.SetActive(false);
        }
    }

    public void StartBossFight()
    {
        if (healthSlider != null)
            healthSlider.gameObject.SetActive(true);

        if (bossBehaviour != null)
            bossBehaviour.StartBossFight();
    }

    // COMPATIBILIDAD:
    // otros scripts seguramente siguen llamando a este método
public void TriggerStunnedState()
{
    BossBehaviour boss = GetComponent<BossBehaviour>();
    if (boss != null)
    {
        boss.TriggerStunnedState();
    }
}

    // Si algún objeto sigue llamando al daño aquí, lo reenviamos también
    public void TakeDamage(int damage)
    {
        if (bossBehaviour != null)
        {
            bossBehaviour.TakeDamage(damage);
        }

        // Opcional: si quieres que la barra refleje vidas del boss
        if (bossBehaviour != null && healthSlider != null)
        {
            float percent = 1f - ((float)bossBehaviour.currentLives / Mathf.Max(1, bossBehaviour.maxLives));
            healthSlider.value = percent * maxHealth;
        }
    }

    private void Die()
    {
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        Debug.Log("BossHealthController: vida agotada");
    }
}