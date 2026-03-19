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

        if (attackController != null)
            attackController.StopAttacking();
    }

    public void PlayIntroAnimation()
    {
        fightStarted = false;
        introPlaying = true;

        if (attackController != null)
            attackController.StopAttacking();

        if (animator != null)
            animator.SetTrigger(introTriggerName);

        if (musicaintro != null && musica != null)
            musicaintro.PlayOneShot(musica);
    }

    // Animation Event al final de la intro
public void OnIntroAnimationFinished()
{
    fightStarted = true; // Esto desbloquea la condición 2
    
    if (attackController != null)
    {
        attackController.canAttack = true; // Esto desbloquea la condición 3
        attackController.StartAttacking(); // Esto reinicia el timer
        
    }
    if (introTrigger != null)
    {
        introTrigger.FinishIntro(); 
    }
}
}