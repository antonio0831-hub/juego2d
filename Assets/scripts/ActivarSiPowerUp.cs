using UnityEngine;

public class ActivarSoloSiPowerUpCogido : MonoBehaviour
{
    private void Start()
    {
        if (PowerUpExplicacion.mostrarObjetoEnSiguienteNivel)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}