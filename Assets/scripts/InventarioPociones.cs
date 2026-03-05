using UnityEngine;
using TMPro; // Necesario para el texto del Canvas
using UnityEngine.UI;

public class InventarioPociones : MonoBehaviour
{
    [System.Serializable]
    public class TipoPocion
    {
        public string nombre;
        public string tagPocion;
        public int cantidad;
        public Sprite icono;
        public int poderCurativo;
        // Aquí puedes añadir más efectos como: public float boostVelocidad;
    }

    [Header("Configuración de Pociones")]
    public TipoPocion[] misPociones;
    private int indiceSeleccionado = 0;

    [Header("Referencias UI")]
    public TextMeshProUGUI textoNombrePocion;
    public TextMeshProUGUI textoCantidad;
    public Image imagenIconoUI;

    private vida scriptVida;

    void Start()
    {
        scriptVida = GetComponent<vida>(); // Buscamos el script de vida en el jugador
        ActualizarInterfaz();
    }

    void Update()
    {
        // Tecla rápida para cambiar de poción (opcional, también puedes usar botones)
        if (Input.GetKeyDown(KeyCode.R)) CambiarSiguientePocion();
        
        // Tecla rápida para usar poción
        if (Input.GetKeyDown(KeyCode.F)) UsarPocionSeleccionada();
    }

    // --- RECOGIDA DE POCIONES ---
    private void OnTriggerEnter2D(Collider2D collision)
    {
        for (int i = 0; i < misPociones.Length; i++)
        {
            if (collision.CompareTag(misPociones[i].tagPocion))
            {
                misPociones[i].cantidad++;
                Destroy(collision.gameObject);
                ActualizarInterfaz();
                break;
            }
        }
    }

    // --- LÓGICA DE INTERFAZ ---
    public void CambiarSiguientePocion()
    {
        indiceSeleccionado++;
        if (indiceSeleccionado >= misPociones.Length) indiceSeleccionado = 0;
        ActualizarInterfaz();
    }

    void ActualizarInterfaz()
    {
        if (misPociones.Length == 0) return;

        TipoPocion actual = misPociones[indiceSeleccionado];
        if (textoNombrePocion != null) textoNombrePocion.text = actual.nombre;
        if (textoCantidad != null) textoCantidad.text = "x" + actual.cantidad.ToString();
        if (imagenIconoUI != null) imagenIconoUI.sprite = actual.icono;
    }

    // --- USO DE POCIONES ---
    public void UsarPocionSeleccionada()
    {
        TipoPocion actual = misPociones[indiceSeleccionado];

        if (actual.cantidad > 0)
        {
            // Si es una poción de vida, llamamos a HealOneHeart de tu script 'vida'
            if (actual.poderCurativo > 0)
            {
                scriptVida.HealOneHeart(); //
            }

            actual.cantidad--;
            ActualizarInterfaz();
            Debug.Log("Usaste " + actual.nombre);
        }
        else
        {
            Debug.Log("No tienes más " + actual.nombre);
        }
    }
}