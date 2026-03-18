using UnityEngine;
using UnityEngine.UI;

public class contadorUIBinder : MonoBehaviour
{
    public Text textoContador;

    private void Start()
    {
        if (contadorManager.instancia != null && textoContador != null)
        {
            contadorManager.instancia.textoContador = textoContador;
            contadorManager.instancia.RefrescarUI();
        }
    }
}