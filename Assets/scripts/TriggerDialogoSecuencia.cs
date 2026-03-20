using System.Collections;
using UnityEngine;

public class TriggerDialogoSecuencia : MonoBehaviour
{
    [Header("Jugador")]
    public string playerTag = "Player";
    public movimiento scriptMovimientoJugador;

    [Header("Diálogos en orden")]
    public GameObject[] dialogos;

    [Header("Tiempo que dura cada diálogo")]
    public float[] duraciones;

    [Header("Opciones")]
    public bool desactivarTriggerAlFinal = true;
    public bool destruirTriggerAlFinal = false;

    private bool activado = false;

    private void Start()
    {
        // Asegura que todos los diálogos empiecen apagados
        for (int i = 0; i < dialogos.Length; i++)
        {
            if (dialogos[i] != null)
                dialogos[i].SetActive(false);
        }

        // Si no asignaste el jugador manualmente, lo busca
        if (scriptMovimientoJugador == null)
scriptMovimientoJugador = Object.FindFirstObjectByType<movimiento>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (activado) return;

        if (collision.CompareTag(playerTag))
        {
            StartCoroutine(SecuenciaDialogos());
        }
    }

    IEnumerator SecuenciaDialogos()
    {
        activado = true;

        // Bloquear movimiento
        if (scriptMovimientoJugador != null)
            scriptMovimientoJugador.puedeMoverse = false;

        // Mostrar diálogos uno por uno
        for (int i = 0; i < dialogos.Length; i++)
        {
            if (dialogos[i] == null) continue;

            dialogos[i].SetActive(true);

            float tiempo = 2f;
            if (duraciones != null && i < duraciones.Length)
                tiempo = duraciones[i];

            yield return new WaitForSeconds(tiempo);

            dialogos[i].SetActive(false);
        }

        // Desbloquear movimiento
        if (scriptMovimientoJugador != null)
            scriptMovimientoJugador.puedeMoverse = true;

        // Desactivar o destruir trigger
        if (destruirTriggerAlFinal)
        {
            Destroy(gameObject);
        }
        else if (desactivarTriggerAlFinal)
        {
            gameObject.SetActive(false);
        }
    }
}