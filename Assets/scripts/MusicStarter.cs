using UnityEngine;

public class MusicStarter : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip levelMusic;

    private void Start()
    {
        if (audioSource != null && levelMusic != null)
        {
            audioSource.clip = levelMusic;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
}