using UnityEngine;

public class tps : MonoBehaviour
{
    [Header("Jugador a teletransportar")]
    public Transform jugador;

    [Header("Punto de destino")]
    public Transform destino;

    private Rigidbody2D Rigidbody2D;

    private void Start()
    {
        Rigidbody2D = jugador.GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Funciona");
        if (other.transform == jugador)
        {
            // Desactivar velocidad para evitar empujones
            Rigidbody2D.linearVelocity = Vector2.zero;
            Rigidbody2D.angularVelocity = 0f;

            // Teletransportar
            jugador.position = destino.position;
        }
    }
}