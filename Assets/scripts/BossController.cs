using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Estado")]
    public bool fightStarted = false;

    [Header("Animación")]
    public Animator animator;
    public string introTriggerName = "Intro";
    public AudioSource musicaintro;
    public AudioClip musica;

    [Header("Referencia opcional al trigger")]
    public BossIntroTrigger introTrigger;

    public void PlayIntroAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger(introTriggerName);
            musicaintro.PlayOneShot(musica);
        }
            
    }

    // ESTA es la función que llamará el Animation Event
    public void OnIntroAnimationFinished()
    {
        if (fightStarted) return;

        fightStarted = true;

        if (introTrigger != null)
            introTrigger.OnBossIntroFinished();
    }

    private void Update()
    {
        if (!fightStarted) return;

        // IA del boss aquí
    }
}