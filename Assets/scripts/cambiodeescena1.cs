using UnityEngine;
using UnityEngine.SceneManagement;
public class cambiodeescena1 : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            SceneManager.LoadScene("Level3");
        }
    }
}

