using UnityEngine;

public class BossMusicTrigger : MonoBehaviour
{
    [Header("Referencias")]
    public MusicFader fader;      // Arrastra aquí el objeto que tiene el MusicFader
    public AudioClip bossMusic;   // Arrastra aquí el archivo de audio del boss

    [Header("Tiempos de Fade")]
    public float fadeOut = 1.5f;
    public float fadeIn = 1.5f;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Si el jugador entra y no hemos activado la música aún
        if (other.CompareTag("Player") && !triggered)
        {
            if (fader != null && bossMusic != null)
            {
                fader.ChangeMusic(bossMusic, fadeOut, fadeIn);
                triggered = true; // Para que no se reinicie la canción si entras y sales
            }
        }
    }
}