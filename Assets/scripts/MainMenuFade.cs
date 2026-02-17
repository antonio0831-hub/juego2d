using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenuFade : MonoBehaviour
{
    [Header("Panel de menú")]
    public GameObject menuPanel;

    [Header("Botones")]
    public Button startButton;
    public Button quitButton;

    [Header("Fade Panel")]
    public Image fadePanel;   // el panel negro para fade
    public float fadeDuration = 1f;

    private void Start()
    {
        // Mostrar menú
        if (menuPanel != null)
            menuPanel.SetActive(true);

        // Conectar botones
        if (startButton != null)
            startButton.onClick.AddListener(StartGame);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);

        // Iniciar fade-in (de negro a transparente)
        if (fadePanel != null)
            StartCoroutine(Fade(1f, 0f));
    }

    public void StartGame()
    {
        // Iniciar fade-out y luego cargar escena
        if (fadePanel != null)
            StartCoroutine(FadeAndLoadScene());
        else
            SceneManager.LoadScene("Level1"); // fallback
    }

    private IEnumerator FadeAndLoadScene()
    {
        // Fade de transparente a negro
        yield return StartCoroutine(Fade(0f, 1f));

        // Carga escena de juego
        SceneManager.LoadScene("Level1");
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

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Juego cerrado");
    }
}