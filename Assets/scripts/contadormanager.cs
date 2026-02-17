using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events; // Necesario para crear eventos personalizados
using System.Collections.Generic; // Necesario para usar Listas

public class contadorManager : MonoBehaviour
{
    [System.Serializable]
    public class EventoEspecial
    {
        public string nombreDelEvento; // Solo para organizar en el inspector
        public int metaDeColeccionables; // A cuántos coleccionables se activa
        public UnityEvent accionAlCompletar; // Qué pasará cuando se alcance la meta
        [HideInInspector] public bool completado = false; // Para que no se repita
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
            // Si llegamos a la meta y el evento no se ha disparado aún
            if (contador >= evento.metaDeColeccionables && !evento.completado)
            {
                evento.accionAlCompletar.Invoke(); // Ejecuta lo que pusiste en el Inspector
                evento.completado = true; // Marca como hecho
                Debug.Log("Evento activado: " + evento.nombreDelEvento);
            }
        }
    }
}