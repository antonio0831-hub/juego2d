using UnityEngine;

public class portal : MonoBehaviour
{
    private Animator portal1;

    void Awake()
    {
        // Obtiene el Animator que está en este mismo objeto
        portal1 = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Detectamos si lo que entró es el Jugador (asegúrate que tu personaje tenga el Tag "Player")
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Jugador detectado por " + gameObject.name);
            portal1.Play("portal"); // Pon aquí el nombre exacto de tu animación
        }
    }
}