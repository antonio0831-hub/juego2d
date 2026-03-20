using UnityEngine;
using System.Collections;

public class BossWeakObject : MonoBehaviour
{
    public BossWeakObjectManager manager;
    public Transform[] possiblePositions;
    private int hitCount = 0;
    private bool activeObject = false;
    private bool isMoving = false;

    public void ActivateObject()
    {
        hitCount = 0; // Resetear contador solo al activarse de verdad
        activeObject = true;
        isMoving = false;
        gameObject.SetActive(true);
    }

    public void DeactivateObject()
    {
        activeObject = false;
        gameObject.SetActive(false);
    }

    public void ReceiveHit()
    {
        if (!activeObject || isMoving) return;

        hitCount++;
        Debug.Log($"<color=yellow>GOLPE en {gameObject.name}:</color> Contador = {hitCount}");

        if (hitCount == 1)
        {
            StartCoroutine(MoveRoutine());
        }
        else if (hitCount >= 2)
        {
            activeObject = false;
            manager.OnWeakObjectDestroyed(this);
            gameObject.SetActive(false);
        }
    }

    IEnumerator MoveRoutine()
    {
        isMoving = true;
        ChangePosition();
        yield return new WaitForSeconds(0.3f); // Tiempo de espera para evitar dobles toques
        isMoving = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) ReceiveHit();
    }

    private void ChangePosition()
    {
        if (possiblePositions.Length > 0)
        {
            int index = Random.Range(0, possiblePositions.Length);
            transform.position = possiblePositions[index].position;
        }
    }
}