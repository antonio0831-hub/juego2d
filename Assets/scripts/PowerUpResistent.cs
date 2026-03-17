using UnityEngine;

public class PowerUpResistent : MonoBehaviour
{
    public static bool tieneAtaqueDistancia = false;

    [Header("Opcional")]
    public AudioClip sonidoRecogida;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        tieneAtaqueDistancia = true;
        Debug.Log("Ataque a distancia desbloqueado para la siguiente escena.");

        if (sonidoRecogida != null)
            AudioSource.PlayClipAtPoint(sonidoRecogida, transform.position);

        Destroy(gameObject);
    }
}