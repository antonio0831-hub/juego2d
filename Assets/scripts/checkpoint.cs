using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public string playerTag = "Player";
    public Color activeColor = Color.green;
    private bool activated = false;
    public AudioSource check;
    public AudioClip checkpoints;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
            vida playerHealth = collision.GetComponentInParent<vida>();

            if (playerHealth != null)
            {
                // Actualizamos siempre el punto de respawn en el script del jugador
                playerHealth.SetCheckpoint(this.transform);

                if (!activated)
                {
                    if (check != null && checkpoints != null) check.PlayOneShot(checkpoints);
                    playerHealth.HealOneHeart(); // Cura un corazón al tocarlo
                    activated = true;
                    if (GetComponent<SpriteRenderer>() != null)
                        GetComponent<SpriteRenderer>().color = activeColor;
                }
            }
        }
    }
}