using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public GameObject tutorialImage;
    private bool tutorialActive = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !tutorialActive)
        {
            ShowTutorial();
        }
    }

    void Update()
    {
        if (tutorialActive && Input.anyKeyDown)
        {
            HideTutorial();
        }
    }

    void ShowTutorial()
    {
        tutorialImage.SetActive(true);
        Time.timeScale = 0f; 
        tutorialActive = true;
    }

    void HideTutorial()
    {
        tutorialImage.SetActive(false);
        Time.timeScale = 1f; 
        tutorialActive = false;
    }
}