using UnityEngine;

public class BossWeakObjectManager : MonoBehaviour
{
    public BossWeakObject object1;
    public BossWeakObject object2;
    public BossBehaviour boss;

    private int currentStep = 0;
    private bool cycleLocked = false;
    private bool fightIsActive = false; // Nueva variable de control

    public void StartNewCycle()
    {
        // Si la pelea ya empezó y no hemos terminado con el objeto 2, bloqueamos el reinicio
        if (fightIsActive && currentStep < 2) 
        {
            Debug.LogWarning("<color=orange>MANAGER:</color> Se intentó reiniciar el ciclo, pero ya hay uno en curso. Ignorado.");
            return;
        }

        fightIsActive = true;
        cycleLocked = false;
        currentStep = 0;

        if (object1 != null) object1.ActivateObject();
        if (object2 != null) object2.DeactivateObject();

        Debug.Log("<color=blue>NUEVO CICLO INICIADO REALMENTE</color>");
    }

    public void OnWeakObjectDestroyed(BossWeakObject destroyedObject)
    {
        if (cycleLocked) return;

        if (destroyedObject == object1 && currentStep == 0)
        {
            currentStep = 1;
            object1.DeactivateObject();
            if (object2 != null) object2.ActivateObject();
            Debug.Log("Objeto 1 fuera. Activando Objeto 2.");
        }
        else if (destroyedObject == object2 && currentStep == 1)
        {
            currentStep = 2; // Marcamos como terminado
            Debug.Log("¡Objeto 2 fuera! Daño al boss.");
            
            if (boss != null)
            {
                boss.TakeBossDamage(2);
                if (boss.currentLives > 0)
                {
                    // Solo aquí permitimos que se limpie el estado para el siguiente ciclo
                    currentStep = 3; 
                    StartNewCycle();
                }
            }
        }
    }

    public void HideAllObjects()
    {
        fightIsActive = false;
        cycleLocked = true;
        if (object1 != null) object1.DeactivateObject();
        if (object2 != null) object2.DeactivateObject();
    }
}