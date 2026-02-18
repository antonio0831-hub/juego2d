using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections; // Necesario para las Corrutinas (WaitForSeconds)
using System.Collections.Generic;

public class contadorManager : MonoBehaviour
{
    [System.Serializable]
    public class EventoEspecial
    {
        public string nombreDelEvento;
        public int metaDeColeccionables;
        
        [Header("Acciones")]
        public UnityEvent accionAlActivar;    // Lo que pasa al empezar
        public float duracionDelEvento = 3f;  // Cuánto tiempo dura activo
        public UnityEvent accionAlTerminar;   // Lo que pasa al finalizar el tiempo

        [HideInInspector] public bool completado = false;
    }

    [Header("Configuración del Conteo")]
    public int contador = 0;
    public Text textoContador;

    [Header("Lista de Eventos Especiales")]
    public List<EventoEspecial> listaDeEventos;

    void Start()
    {
        ActualizarInterfaz();
    }

    public void SumarColeccionable()
    {
        contador++;
        ActualizarInterfaz();
        ComprobarEventos();
    }

    void ActualizarInterfaz()
    {
        if (textoContador != null) textoContador.text = "x " + contador.ToString();
    }

    void ComprobarEventos()
    {
        foreach (EventoEspecial evento in listaDeEventos)
        {
            if (contador >= evento.metaDeColeccionables && !evento.completado)
            {
                // Iniciamos la secuencia temporal
                StartCoroutine(EjecutarEventoConDuracion(evento));
                evento.completado = true;
            }
        }
    }

    // Esta función maneja el tiempo de espera
    IEnumerator EjecutarEventoConDuracion(EventoEspecial ev)
    {
        Debug.Log("Iniciando evento: " + ev.nombreDelEvento);
        
        // 1. Ejecutar lo que configuraste en el Inspector (ej: Activar un texto o animación)
        ev.accionAlActivar.Invoke();

        // 2. Esperar el tiempo definido
        yield return new WaitForSeconds(ev.duracionDelEvento);

        // 3. Ejecutar la acción de limpieza (ej: Desactivar el texto o volver a la normalidad)
        ev.accionAlTerminar.Invoke();
        
        Debug.Log("Evento finalizado: " + ev.nombreDelEvento);
    }
}