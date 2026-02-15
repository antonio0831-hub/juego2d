using UnityEngine;

public class checkpoint : MonoBehaviour
{
    public string playerTag = "Player";

    [Header("Opcional: desactivar tras activarse")]
    public bool disableAfterUse = true;

    private bool activated;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (activated) return;
        if (!other.CompareTag(playerTag)) return;

        vida v = other.GetComponentInParent<vida>();
        if (v != null)
        {
            v.SetCheckpoint(transform);
            activated = true;

            if (disableAfterUse)
                GetComponent<Collider2D>().enabled = false;
        }
    }
}
