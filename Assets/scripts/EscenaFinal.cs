using UnityEngine;
using UnityEngine.SceneManagement;
public class EscenaFinal : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
public void Nivel1()
{
    SceneManager.LoadScene("Level1");
}
public void Nivel2()
{
    SceneManager.LoadScene("Level2");
}
public void Nivel3()
{
    SceneManager.LoadScene("Level3");
}
public void SalirDelJuego()
{
    Application.Quit();
}
public void ElegirEscenario()
{
    SceneManager.LoadScene("ElegirEscenario");
}
}
