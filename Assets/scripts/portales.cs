using UnityEngine;

public class portales : MonoBehaviour
{
    public Animator portal1, portal2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
 private void OnTriggerEnter2D(Collider2D collision)
 {
    if(collision.CompareTag("portal1"))
    {
        portal1.Play("portal");
    }
    if(collision.CompareTag("portal2"))
    {
        portal2.Play("portal2");
    }
 }
}
