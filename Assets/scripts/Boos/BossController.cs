using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Estado")]
    public bool fightStarted = false;
    public bool introPlaying = false;

    [Header("Animación")]
    public Animator animator;
    public string introTriggerName = "Intro";
    public AudioSource musicaintro;
    public AudioClip musica;

    [Header("Referencias")]
    public BossIntroTrigger introTrigger;
    public BossBehaviour bossBehaviour;
    public BossAttackController attackController;

    private void Awake()
    {
        fightStarted = false;
        introPlaying = false;
    }

    private void Start()
    {
        if (attackController != null)
            attackController.StopAttacking();
    }

    public void PlayIntroAnimation()
    {
        if (introPlaying) return;

        fightStarted = false;
        introPlaying = true;

        if (attackController != null)
            attackController.StopAttacking();

        if (animator != null)
            animator.SetTrigger(introTriggerName);

        if (musicaintro != null && musica != null)
            musicaintro.PlayOneShot(musica);
    }

    public void OnIntroAnimationFinished()
    {
        introPlaying = false;
        fightStarted = true;

        if (bossBehaviour != null)
            bossBehaviour.StartBossFight();

        if (introTrigger != null)
            introTrigger.FinishIntro();
    }
}