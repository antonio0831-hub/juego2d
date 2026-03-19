using UnityEngine;

public class BossWeakObjectManager : MonoBehaviour
{
    [Header("Objetos")]
    public BossWeakObject object1;
    public BossWeakObject object2;

    [Header("Boss")]
    public BossBehaviour boss; 

    private int currentStep = 0;

    private void Awake()
    {
        if (boss == null)
        {
            boss = Object.FindFirstObjectByType<BossBehaviour>();
        }
    }

    private void Start()
    {
        StartNewCycle();
    }

    public void StartNewCycle()
    {
        currentStep = 0;
        if (object1 != null) object1.ActivateObject();
        if (object2 != null) object2.DeactivateObject();
    }

    public void OnWeakObjectDestroyed(BossWeakObject destroyedObject)
    {
        if (destroyedObject == object1 && currentStep == 0)
        {
            currentStep = 1;
            if (object2 != null) object2.ActivateObject();
        }
        else if (destroyedObject == object2 && currentStep == 1)
        {
            currentStep = 2;

            Debug.Log("Objeto 2 destruido, intentando stunear boss");

            if (boss != null) 
                boss.TriggerTrapState(); 
            else
                Debug.LogError("ERROR: El Manager no encuentra al BossBehaviour.");
        }
    }
}