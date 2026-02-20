using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TeleportPortal : MonoBehaviour
{
    [Header("Destino")]
    public Transform respawnPoint; // arrastra un Empty del destino
    [Header("Sonidos")]
    public AudioSource tp;
    public AudioClip contacto;

    [Header("Opcional")]
    public bool disableAfterUse = true;  // si quieres que desaparezca tras usarlo
    public string playerTag = "Player";

    void Reset()
    {
        // recomendable: trigger collider
        var col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {

        if (!other.CompareTag(playerTag)) return;
        if (respawnPoint == null) return;
    if (tp != null && contacto != null) 
    {
        tp.PlayOneShot(contacto);
    }
        // mover al jugador
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.position = respawnPoint.position;
        }
        else
        {
            other.transform.position = respawnPoint.position;
        }

        if (disableAfterUse)
            gameObject.SetActive(false);
    }
}
