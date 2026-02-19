using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPausa : MonoBehaviour
{
    [Header("UI del Menú")]
    public GameObject objetoMenuPausa; // El Panel que contiene el menú
    private bool juegoPausado = false;

    void Update()
    {
        // Detecta si pulsas la tecla Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (juegoPausado)
            {
                Reanudar();
            }
            else
            {
                Pausar();
            }
        }
    }

    public void Reanudar()
    {
        objetoMenuPausa.SetActive(false); // Esconde el menú
        Time.timeScale = 1f;              // El tiempo vuelve a correr
        juegoPausado = false;
    }

    void Pausar()
    {
        objetoMenuPausa.SetActive(true);  // Muestra el menú
        Time.timeScale = 0f;              // Congela el juego
        juegoPausado = true;
    }

    public void SalirAlMenuPrincipal()
    {
        Time.timeScale = 1f;
        // Aquí puedes poner el nombre de tu escena de menú principal
        // SceneManager.LoadScene("MenuPrincipal"); 
        
        // Por ahora, usemos tu lógica de salir del juego completa:
            SceneManager.LoadScene("Interfaz");
    }
}