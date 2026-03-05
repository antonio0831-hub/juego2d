using System.Collections;
using UnityEngine;

public class ZonaVeneno2D : MonoBehaviour
{
    public int damagePerTick = 1;
    public float tickInterval = 1f;
    public string playerTag = "Player";

    private Coroutine poisonCoroutine;
    private bool playerInside = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (playerInside) return; // evita duplicados
        playerInside = true;

        vida v = other.GetComponent<vida>();
        if (v == null) v = other.GetComponentInParent<vida>();
        if (v == null) return;

        poisonCoroutine = StartCoroutine(PoisonDamage(v));
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        playerInside = false;

        if (poisonCoroutine != null)
        {
            StopCoroutine(poisonCoroutine);
            poisonCoroutine = null;
        }
    }

    IEnumerator PoisonDamage(vida v)
    {
        while (playerInside && v != null)
        {
            v.TakeDamage(damagePerTick);
            yield return new WaitForSeconds(tickInterval);
        }
    }
}