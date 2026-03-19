using UnityEngine;

public class BossWeakObject : MonoBehaviour
{
    [Header("Manager")]
    public BossWeakObjectManager manager;

    [Header("Posiciones posibles")]
    public Transform[] possiblePositions;

    private int hitCount = 0;
    private bool activeObject = false;

    private void Awake()
    {
        // 🔍 AUTO-BUSQUEDA: Si la casilla está vacía, la busca en la escena
        if (manager == null)
        {
            manager = Object.FindFirstObjectByType<BossWeakObjectManager>();
        }
    }

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
    // 1. Si el objeto ya no está activo para hits, salimos
    if (!activeObject) return;

    // 2. Verificamos el manager ANTES de hacer nada
    if (manager == null)
    {
        // Esto nos dirá en la consola CÓMO SE LLAMA el objeto que no tiene manager
        Debug.LogError($"El objeto '{gameObject.name}' no tiene asignado el BossWeakObjectManager. ¡Bórrale el script o asígnalo!");
        return; 
    }

    hitCount++;
    Debug.Log($"{gameObject.name} golpeado. Intento: {hitCount}");

    if (hitCount == 1)
    {
        ChangePosition();
    }
    else if (hitCount >= 2)
    {
        activeObject = false;
        manager.OnWeakObjectDestroyed(this);
        gameObject.SetActive(false);
    }
}

private void ChangePosition()
{
    if (possiblePositions == null || possiblePositions.Length == 0)
        return;

    Vector3 currentPos = transform.position;
    Transform selected = null;

    int safety = 20;
    while (safety > 0)
    {
        int randomIndex = Random.Range(0, possiblePositions.Length);
        selected = possiblePositions[randomIndex];

        if (selected != null && Vector3.Distance(currentPos, selected.position) > 0.05f)
            break;

        safety--;
    }

    if (selected != null)
        transform.position = selected.position;
}
}