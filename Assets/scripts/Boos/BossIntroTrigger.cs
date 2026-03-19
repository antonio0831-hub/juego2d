using System.Collections;
using UnityEngine;

public class BossIntroTrigger : MonoBehaviour
{
    [Header("Jugador")]
    public MonoBehaviour playerMovement;
    public Rigidbody2D playerRb;

    [Header("Cámaras")]
    public Camera playerCamera;
    public Camera bossIntroCamera;

    [Header("Fade visual")]
    public ScreenFader screenFader;
    public float fadeToBlackTime = 0.8f;
    public float fadeFromBlackTime = 0.8f;

    [Header("Música")]
    public MusicFader musicFader;
    public AudioClip bossMusic;
    public float audioFadeOutTime = 1f;
    public float audioFadeInTime = 1f;

    [Header("Boss")]
    public BossController bossController;

    private bool activated = false;
    private bool introFinished = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activated) return;
        if (!other.CompareTag("Player")) return;

        activated = true;
        StartCoroutine(PlayBossIntro());
    }

    private IEnumerator PlayBossIntro()
    {
        if (playerMovement != null)
            playerMovement.enabled = false;

        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero;
            playerRb.angularVelocity = 0f;
        }

        if (screenFader != null)
            yield return screenFader.FadeInCoroutine(fadeToBlackTime);

        if (playerCamera != null) playerCamera.gameObject.SetActive(false);
        if (bossIntroCamera != null) bossIntroCamera.gameObject.SetActive(true);

      // Por esto
if (musicFader != null && bossMusic != null)
    musicFader.ChangeMusic(bossMusic, audioFadeOutTime, audioFadeInTime);

        if (bossController != null)
            bossController.PlayIntroAnimation();

        if (screenFader != null)
            yield return screenFader.FadeOutCoroutine(fadeFromBlackTime);

        // Espera hasta que la animación dispare el Animation Event
        while (!introFinished)
            yield return null;

        if (playerMovement != null)
            playerMovement.enabled = true;

        gameObject.SetActive(false);
    }

    // Esta función la llama BossController cuando termina la animación
    public void OnBossIntroFinished()
    {
        introFinished = true;
    }
    public void FinishIntro()
{
    introFinished = true;
}
}