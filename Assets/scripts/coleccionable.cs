using UnityEngine;

public class coleccionable : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            contadorManager manager = Object.FindFirstObjectByType<contadorManager>();
            if (manager != null) manager.SumarColeccionable();
            
            Destroy(gameObject);
        }
    }
}