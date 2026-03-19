using UnityEngine;

public class BossWeakObject : MonoBehaviour
{
    [Header("Manager")]
    public BossWeakObjectManager manager;

    [Header("Posiciones posibles")]
    public Transform[] possiblePositions;

    private int hitCount = 0;
    private bool activeObject = false;

    public void ActivateObject()
    {
        hitCount = 0;
        activeObject = true;
        gameObject.SetActive(true);
    }

    public void DeactivateObject()
    {
        activeObject = false;
        gameObject.SetActive(false);
    }

    public void ReceiveHit()
    {
        if (!activeObject) return;

        hitCount++;

        if (hitCount == 1)
        {
            ChangePosition();
        }
        else if (hitCount >= 2)
        {
            activeObject = false;
            gameObject.SetActive(false);

            if (manager != null)
                manager.OnWeakObjectDestroyed(this);
        }
    }

    private void ChangePosition()
    {
        if (possiblePositions == null || possiblePositions.Length == 0)
            return;

        int randomIndex = Random.Range(0, possiblePositions.Length);
        transform.position = possiblePositions[randomIndex].position;
    }
}