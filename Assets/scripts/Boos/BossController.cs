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
        // Aseguramos que el ataque esté apagado al iniciar el nivel
        if (attackController != null)
        {
            attackController.StopAttacking();
        }
    }

    public void PlayIntroAnimation()
    {
        // Si ya está sonando la intro, no hacemos nada
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

    // Se llama mediante Animation Event al final del clip de Intro
    public void OnIntroAnimationFinished()
    {
        introPlaying = false;
        fightStarted = true;

        if (bossBehaviour != null)
            bossBehaviour.StartBossFight();

        if (attackController != null)
        {
            attackController.StartAttacking(); 
        }

        if (introTrigger != null)
            introTrigger.FinishIntro();
    }
}