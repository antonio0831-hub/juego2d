using UnityEngine;

public class portal2 : MonoBehaviour
{
    public Animator portal2s; // Arrastra el Animator de ESTE portal aquí
    public string nombreAnimacion = "portal2"; // Nombre de la animación en el Animator

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Preguntamos si lo que entró al portal es el Jugador
        if(collision.CompareTag("Player")) 
        {
            portal2s.Play(nombreAnimacion);
        }
    }
}