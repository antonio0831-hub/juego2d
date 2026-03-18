using System.Collections;
using UnityEngine;

public class MusicFader : MonoBehaviour
{
    public AudioSource audioSource;
    public float maxVolume = 1f;

    private Coroutine currentRoutine;

    private void Awake()
    {
        if (audioSource != null)
            maxVolume = audioSource.volume;
    }

    public void ChangeMusic(AudioClip newClip, float fadeOutTime, float fadeInTime)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ChangeMusicCoroutine(newClip, fadeOutTime, fadeInTime));
    }

    public IEnumerator ChangeMusicCoroutine(AudioClip newClip, float fadeOutTime, float fadeInTime)
    {
        if (audioSource == null || newClip == null)
            yield break;

        float initialVolume = audioSource.volume;

        // Fade out
        float t = 0f;
        while (t < fadeOutTime)
        {
            t += Time.unscaledDeltaTime;
            audioSource.volume = Mathf.Lerp(initialVolume, 0f, t / fadeOutTime);
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();

        // Cambia clip y reproduce
        audioSource.clip = newClip;
        audioSource.Play();

        // Fade in
        t = 0f;
        while (t < fadeInTime)
        {
            t += Time.unscaledDeltaTime;
            audioSource.volume = Mathf.Lerp(0f, maxVolume, t / fadeInTime);
            yield return null;
        }

        audioSource.volume = maxVolume;
        currentRoutine = null;
    }
}