using UnityEngine;

public class FlightActivator : MonoBehaviour
{
    [Header("Configuración")]
    public string playerTag = "Player"; // Asegúrate de que tu jugador tenga el Tag "Player"
    
    // Al entrar en el Trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Comprobamos si es el jugador
        if (collision.CompareTag(playerTag))
        {
            // Buscamos el componente de vuelo en el jugador
            PlayerFlight flightScript = collision.GetComponent<PlayerFlight>();

            if (flightScript != null)
            {
                // ACTIVAMOS el script
                flightScript.enabled = true;
                
                Debug.Log("¡Vuelo desbloqueado mediante Trigger!");
                
                // Opcional: Destruir este objeto activador tras el uso
                // Destroy(gameObject);
            }
        }
    }
}