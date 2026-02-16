using System.Collections;
using UnityEngine;

public class ataque_especial : MonoBehaviour
{
    [Header("Input")]
    public KeyCode key = KeyCode.Q;

    [Header("Refs")]
    public KillChargeManager charge;     // arrastra el KillChargeManager del Player (o se auto-busca)
    public ataque_distancia ranged;      // arrastra el ataque_distancia del Player
    public Animator anim;                // animator del Player

    [Header("Animator")]
    public string specialTrigger = "ATespecial";

    [Header("Timing")]
    public float animDelayToFire = 0.12f;   // cuando quieres que salga el bolt dentro de la anim
    public float lockTime = 0.35f;          // cuanto bloquea antes de permitir otro input

    private bool busy;

    void Awake()
    {
        if (charge == null) charge = GetComponent<KillChargeManager>();
        if (ranged == null) ranged = GetComponent<ataque_distancia>();
        if (anim == null) anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (busy) return;
        if (charge == null || ranged == null) return;

        // Solo si está cargado
        if (!charge.charged) return;

        if (Input.GetKeyDown(key))
        {
            StartCoroutine(DoSpecial());
        }
    }

    IEnumerator DoSpecial()
    {
        busy = true;

        // Disparar animación
        if (anim != null && !string.IsNullOrEmpty(specialTrigger))
        {
            anim.ResetTrigger(specialTrigger);
            anim.SetTrigger(specialTrigger);
        }

        // Esperar a que cuadre con la animación
        if (animDelayToFire > 0f)
            yield return new WaitForSeconds(animDelayToFire);

        // Disparo (si es recto, pon shotMode=StraightHorizontal en el ataque_distancia del player)
        ranged.Disparar(null);

        // Consumir la carga justo cuando se usa
        charge.Consume();

        // Bloqueo pequeño para evitar spam
        if (lockTime > 0f)
            yield return new WaitForSeconds(lockTime);

        busy = false;
    }
}
