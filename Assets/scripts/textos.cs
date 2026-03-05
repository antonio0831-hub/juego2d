using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class textos : MonoBehaviour
{
    [System.Serializable]
    public class EventoEspecial
    {
        public string nombreDelEvento;
        public UnityEvent accionAlActivar;    // Lo que pasa al entrar en el trigger
        public float duracionDelEvento = 3f;  // Cuánto tiempo dura activo
        public UnityEvent accionAlTerminar;   // Lo que pasa al finalizar el tiempo
        [HideInInspector] public bool completado = false;
    }

    public List<EventoEspecial> listaDeEventos;
    public string tagJugador = "Player"; // Solo el jugador activará el evento

    // Se activa automáticamente al entrar en el trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(tagJugador))
        {
            ActivarSiguienteEvento();
        }
    }

    public void ActivarSiguienteEvento()
    {
        foreach (EventoEspecial evento in listaDeEventos)
        {
            if (!evento.completado)
            {
                StartCoroutine(EjecutarEventoConDuracion(evento));
                evento.completado = true;
                break; // Solo activa uno por cada vez que tocas el trigger
            }
        }
    }

    IEnumerator EjecutarEventoConDuracion(EventoEspecial ev)
    {
        Debug.Log("Iniciando evento: " + ev.nombreDelEvento);
        
        // 1. Ejecutar lo que configuraste en el Inspector (ej: Activar un texto)
        ev.accionAlActivar.Invoke();

        // 2. Esperar el tiempo exacto definido para ESTE evento
        yield return new WaitForSeconds(ev.duracionDelEvento);

        // 3. Ejecutar la acción de limpieza (ej: Desactivar el texto)
        ev.accionAlTerminar.Invoke();
        
        Debug.Log("Evento finalizado: " + ev.nombreDelEvento);
    }
}