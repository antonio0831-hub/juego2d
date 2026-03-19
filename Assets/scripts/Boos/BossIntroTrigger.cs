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
    public BossHealthController bossHealth;
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
        // 1. Bloquear jugador
        if (playerMovement != null) playerMovement.enabled = false;
        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero;
            playerRb.angularVelocity = 0f;
        }

        // 2. Fade Out (Pantalla a negro)
        if (screenFader != null)
            yield return screenFader.FadeInCoroutine(fadeToBlackTime);

        // 3. Cambiar cámaras y música
        if (playerCamera != null) playerCamera.gameObject.SetActive(false);
        if (bossIntroCamera != null) bossIntroCamera.gameObject.SetActive(true);

        if (musicFader != null && bossMusic != null)
            musicFader.ChangeMusic(bossMusic, audioFadeOutTime, audioFadeInTime);

        // 4. Activar Boss e iniciar Animación
        if (bossController != null)
        {
            bossController.enabled = true;
            if (bossController.attackController != null) 
                bossController.attackController.enabled = true;

            bossController.PlayIntroAnimation();
        }

        // 5. Fade In (Mostrar escena)
        if (screenFader != null)
            yield return screenFader.FadeOutCoroutine(fadeFromBlackTime);

        // 6. Esperar a que el Animation Event del Boss Controller diga que terminó
        while (!introFinished)
            yield return null;

        // 7. Devolver control al jugador
        if (playerMovement != null)
            playerMovement.enabled = true;

        gameObject.SetActive(false);
    }

    public void FinishIntro()
    {
        introFinished = true;
    }
}