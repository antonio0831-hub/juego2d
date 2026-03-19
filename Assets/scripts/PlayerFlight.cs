using UnityEngine;
using System.Collections;

public class PlayerFlight : MonoBehaviour
{
    [Header("Configuración de Vuelo")]
    public float flightSpeed = 10f;       // Velocidad al volar
    public float flightDuration = 5f;    // Cuánto tiempo dura el vuelo
    public KeyCode flightKey = KeyCode.V; // Tecla para activar

    [Header("Referencias")]
    public Rigidbody2D rb;

    private bool isFlying = false;
    private float originalGravity;

    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        
        // Guardamos la gravedad original para devolverla al terminar
        originalGravity = rb.gravityScale;
    }

    void Update()
    {
        // Detectar si presionamos la tecla y no estamos volando ya
        if (Input.GetKeyDown(flightKey) && !isFlying)
        {
            StartCoroutine(FlyRoutine());
        }

        // Si estamos volando, procesamos el movimiento libre
        if (isFlying)
        {
            MovePlayerFree();
        }
    }

    void MovePlayerFree()
    {
        // Obtenemos input de las flechas o WASD
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // Aplicamos velocidad directamente en X e Y
        rb.linearVelocity = new Vector2(h, v).normalized * flightSpeed;
    }

    IEnumerator FlyRoutine()
    {
        isFlying = true;
        rb.gravityScale = 0; // Quitamos la gravedad
        
        Debug.Log("¡Vuelo activado!");

        // Esperamos los segundos que configuraste
        yield return new WaitForSeconds(flightDuration);

        // Restauramos todo
        isFlying = false;
        rb.gravityScale = originalGravity;
        
        Debug.Log("Vuelo terminado. Volviendo a la normalidad.");
    }
}