using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameFlowManager : MonoBehaviour
{
    [Header("UI Menú")]
    public GameObject menuPanel;
    public Button startButton;
    public Button quitButton;

    [Header("Fade")]
    public Image fadePanel;
    public float fadeDuration = 1f;

    [Header("Jugador")]
    public movimiento player;

    private void Awake()
    {
        // Conectar botones
        if (startButton != null)
            startButton.onClick.AddListener(() => StartCoroutine(StartGameFade()));

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);

        // Fade inicial
        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(true);
            StartCoroutine(Fade(1f, 0f));
        }
    }

    // ─── INICIAR JUEGO ───
    private IEnumerator StartGameFade()
    {
        if (fadePanel != null)
        {
            yield return StartCoroutine(Fade(0f, 1f));
        }

        menuPanel.SetActive(false);

        // Fade de nuevo a transparente
        if (fadePanel != null)
            yield return StartCoroutine(Fade(1f, 0f));
    }
// ─── MORIR Y REINICIAR JUGADOR ───
    public IEnumerator PlayerDied()
    {
        if (fadePanel != null)
            yield return StartCoroutine(Fade(0f, 1f));
        if (fadePanel != null)
            yield return StartCoroutine(Fade(1f, 0f));
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;
        Color color = fadePanel.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            fadePanel.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        fadePanel.color = new Color(color.r, color.g, color.b, endAlpha);
    }

    private void QuitGame()
    {
        Application.Quit();
        Debug.Log("Juego cerrado");
    }
}