using UnityEngine;

public class PlayerPowerUps : MonoBehaviour
{
    public static bool vidaExtraPendiente = false;

    public AudioClip sonidoRecogida;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        vidaExtraPendiente = true;

        if (sonidoRecogida != null)
            AudioSource.PlayClipAtPoint(sonidoRecogida, transform.position);

        Destroy(gameObject);
    }
}