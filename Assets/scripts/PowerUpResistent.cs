using UnityEngine;

public class PowerUpPersistent : MonoBehaviour
{
    // Esta variable es la clave: al ser 'static' no se reinicia al cambiar de escena
    public static bool tienePoderEspecial = false;

    [Header("Efectos")]
    public AudioClip sonidoRecogida;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            tienePoderEspecial = true; // Activamos el poder para el futuro
            Debug.Log("Poder especial guardado para la siguiente escena.");

            if (sonidoRecogida != null)
            {
                AudioSource.PlayClipAtPoint(sonidoRecogida, transform.position);
            }

            Destroy(gameObject); // El objeto desaparece al recogerlo
        }
    }
}
