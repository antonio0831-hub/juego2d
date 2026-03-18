using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    public Image fadeImage;
    public float defaultFadeTime = 1f;

    private Coroutine currentFade;

    private void Awake()
    {
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
            fadeImage.raycastTarget = false;
        }
    }

    public void FadeIn(float duration)
    {
        // Fade a negro
        StartFade(1f, duration);
    }

    public void FadeOut(float duration)
    {
        // De negro a transparente
        StartFade(0f, duration);
    }

    public IEnumerator FadeInCoroutine(float duration)
    {
        yield return FadeToAlpha(1f, duration);
    }

    public IEnumerator FadeOutCoroutine(float duration)
    {
        yield return FadeToAlpha(0f, duration);
    }

    private void StartFade(float targetAlpha, float duration)
    {
        if (currentFade != null)
            StopCoroutine(currentFade);

        currentFade = StartCoroutine(FadeToAlpha(targetAlpha, duration));
    }

    private IEnumerator FadeToAlpha(float targetAlpha, float duration)
    {
        if (fadeImage == null)
            yield break;

        Color color = fadeImage.color;
        float startAlpha = color.a;
        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(time / duration);

            color.a = Mathf.Lerp(startAlpha, targetAlpha, t);
            fadeImage.color = color;

            yield return null;
        }

        color.a = targetAlpha;
        fadeImage.color = color;
        currentFade = null;
    }
}