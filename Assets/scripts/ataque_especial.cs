using System.Collections;
using UnityEngine;

public class ataque_especial : MonoBehaviour
{
    public KeyCode key = KeyCode.Q;
    public KillChargeManager charge;     
    public ataque_distancia ranged;      

    void Update()
    {
        // Si sale el error de la imagen, esto nos dirá quién es el culpable
        if (charge == null) { 
            Debug.LogError("Culpable: No has arrastrado el KillChargeManager al inspector!"); 
            return; 
        }
        if (ranged == null) { 
            Debug.LogError("Culpable: No has arrastrado el ataque_distancia al inspector!"); 
            return; 
        }

        if (charge.charged && Input.GetKeyDown(key))
        {
            StartCoroutine(DoSpecial());
        }
    }

    IEnumerator DoSpecial()
    {
        if (ranged != null) ranged.Disparar(null);
        if (charge != null) charge.Consume();
        yield return null;
    }
}