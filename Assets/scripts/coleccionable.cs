using UnityEngine;

public class coleccionable : MonoBehaviour
{
    [Header("Sonidos")]
    public AudioSource fuenteDeAudio;
    public AudioClip sonidoAtaque;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
             fuenteDeAudio.PlayOneShot(sonidoAtaque);
            contadorManager manager = Object.FindFirstObjectByType<contadorManager>();
            if (manager != null) manager.SumarColeccionable();
            Destroy(gameObject);
        }
    }
}