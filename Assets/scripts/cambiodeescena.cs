using UnityEngine;
using UnityEngine.SceneManagement;

public class cambiodeescena : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            // Esto te ayudará a ver en el log si el trigger se activa
        vida.ResetCheckpointGlobal(); // <--- ESTO LIMPIA LA POSICIÓN            
            // Usar el índice suele ser más seguro en Builds
            SceneManager.LoadScene("Level2"); 
        }
    }
}