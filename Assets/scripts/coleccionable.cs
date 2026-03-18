using UnityEngine;

public class coleccionable : MonoBehaviour
{
    [Header("Sonidos")]
    public AudioSource fuenteDeAudio;
    public AudioClip sonidoAtaque;

    private bool recogido = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (recogido) return;
        if (!collision.CompareTag("Player")) return;

        recogido = true;

        if (fuenteDeAudio != null && sonidoAtaque != null)
            fuenteDeAudio.PlayOneShot(sonidoAtaque);

        if (contadorManager.instancia != null)
            contadorManager.instancia.SumarColeccionable();

        Destroy(gameObject);
    }
}