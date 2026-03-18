using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class contadorManager : MonoBehaviour
{
    [System.Serializable]
    public class EventoEspecial
    {
        public string nombreDelEvento;
        public int metaDeColeccionables;

        [Header("Acciones")]
        public UnityEvent accionAlActivar;
        public float duracionDelEvento = 3f;
        public UnityEvent accionAlTerminar;

        [HideInInspector] public bool completado = false;
    }

    public static contadorManager instancia;
    public static int contadorGlobal = 0;

    [Header("Configuración del Conteo")]
    public Text textoContador;

    [Header("Lista de Eventos Especiales")]
    public List<EventoEspecial> listaDeEventos;

    private void Awake()
    {
        // Si ya existe uno, destruimos este duplicado
        if (instancia != null && instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        instancia = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        ActualizarInterfaz();
        ComprobarEventos();
    }

    private void OnEnable()
    {
        ActualizarInterfaz();
    }

    public void SumarColeccionable()
    {
        contadorGlobal++;
        ActualizarInterfaz();
        ComprobarEventos();
    }

    public void RestarColeccionable(int cantidad)
    {
        contadorGlobal -= cantidad;
        if (contadorGlobal < 0) contadorGlobal = 0;

        ActualizarInterfaz();
        ComprobarEventos();
    }

    public void ReiniciarContador()
    {
        contadorGlobal = 0;

        foreach (EventoEspecial evento in listaDeEventos)
        {
            evento.completado = false;
        }

        ActualizarInterfaz();
    }

    void ActualizarInterfaz()
    {
        if (textoContador != null)
            textoContador.text = "x " + contadorGlobal.ToString();
    }

    void ComprobarEventos()
    {
        foreach (EventoEspecial evento in listaDeEventos)
        {
            if (contadorGlobal >= evento.metaDeColeccionables && !evento.completado)
            {
                StartCoroutine(EjecutarEventoConDuracion(evento));
                evento.completado = true;
            }
        }
    }

    IEnumerator EjecutarEventoConDuracion(EventoEspecial ev)
    {
        Debug.Log("Iniciando evento: " + ev.nombreDelEvento);

        ev.accionAlActivar.Invoke();

        yield return new WaitForSeconds(ev.duracionDelEvento);

        ev.accionAlTerminar.Invoke();

        Debug.Log("Evento finalizado: " + ev.nombreDelEvento);
    }

    // Útil para actualizar el texto manualmente al cargar una escena nueva
    public void RefrescarUI()
    {
        ActualizarInterfaz();
    }
}