using UnityEngine;

public class PowerUpExplicacion : MonoBehaviour
{
    public static bool mostrarObjetoEnSiguienteNivel = false;

    [Header("Opcional")]
    public AudioClip sonidoRecogida;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        mostrarObjetoEnSiguienteNivel = true;
        Debug.Log("Se mostrará el objeto especial en el siguiente nivel.");

        if (sonidoRecogida != null)
            AudioSource.PlayClipAtPoint(sonidoRecogida, transform.position);

        Destroy(gameObject);
    }
}