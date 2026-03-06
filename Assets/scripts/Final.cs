using UnityEngine;
using UnityEngine.SceneManagement;
public class Final : MonoBehaviour
{
    public GameObject final;
    // Start is called once before the first execution of Update after the MonoBehaviour is create


    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))        
        {
            final.SetActive(true);
             Time.timeScale = 0f; 
        }
    }
}
