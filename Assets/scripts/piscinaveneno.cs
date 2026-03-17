using UnityEngine;

public class piscinaveneno : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        vida playerHealth = other.GetComponentInParent<vida>();

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(playerHealth.maxHealth);
        }
    }
}