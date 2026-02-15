using UnityEngine;

public class camara : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform player;                 // arrastra aquí el player (mejor que GameObject)
    public string playerTag = "Player";      // por si no lo asignas

    [Header("Seguimiento")]
    public Vector3 offset = new Vector3(0f, 0f, -10f); // en 2D suele ser Z = -10
    public float smoothTime = 0.12f;                   // cuanto más alto, más suave/lento

    [Header("Limites (opcional)")]
    public bool useLimits = false;
    public Vector2 minPos; // ej: (-20, -5)
    public Vector2 maxPos; // ej: ( 50, 10)

    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag(playerTag);
            if (p != null) player = p.transform;
        }
    }

    void LateUpdate()
    {
        if (player == null)
        {
            // por si el player aparece después (respawn)
            GameObject p = GameObject.FindGameObjectWithTag(playerTag);
            if (p != null) player = p.transform;
            else return;
        }

    Vector3 targetPos = new Vector3(player.position.x, player.position.y, 0f) + offset;


        // Suavizado estable
        Vector3 newPos = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);

        // Limites opcionales
        if (useLimits)
        {
            newPos.x = Mathf.Clamp(newPos.x, minPos.x, maxPos.x);
            newPos.y = Mathf.Clamp(newPos.y, minPos.y, maxPos.y);
        }

        transform.position = newPos;
    }
}


