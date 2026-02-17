using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Configuración")]
    public string playerTag = "Player";
    public Color activeColor = Color.green;
    private bool activated = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Buscamos al jugador por Tag
        if (collision.CompareTag(playerTag))
        {
            // Buscamos el script vida en el objeto o sus padres
            vida playerHealth = collision.GetComponentInParent<vida>();

            if (playerHealth != null)
            {
                // Actualizamos siempre el punto de respawn
                playerHealth.SetCheckpoint(this.transform);

                // Curamos si es la primera vez que tocamos ESTE checkpoint
                if (!activated)
                {
                    playerHealth.HealOneHeart();
                    activated = true;
                    
                    if (GetComponent<SpriteRenderer>() != null)
                        GetComponent<SpriteRenderer>().color = activeColor;
                    
                    Debug.Log("Checkpoint Activado: Vida aumentada a " + playerHealth.gameObject.name);
                }
            }
        }
    }
}