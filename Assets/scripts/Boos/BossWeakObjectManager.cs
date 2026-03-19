using UnityEngine;

public class BossWeakObjectManager : MonoBehaviour
{
    [Header("Objetos")]
    public BossWeakObject object1;
    public BossWeakObject object2;

    [Header("Boss")]
    // CAMBIO CLAVE: Debe ser BossBehaviour, no BossController
    public BossBehaviour boss; 

    private int currentStep = 0;

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
            // Ahora esta función SÍ existe en BossBehaviour
            if (boss != null) boss.TriggerTrapState(); 
        }
    }
}