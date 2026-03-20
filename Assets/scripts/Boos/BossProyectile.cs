using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 8f;
    public float lifeTime = 4f;

    [Header("Daño")]
    public int damage = 1;

    private Vector2 moveDirection;

    public void SetDirection(Vector2 direction)
    {
        moveDirection = direction.normalized;
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.position += (Vector3)(moveDirection * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        vida playerLife = other.GetComponent<vida>();
        if (playerLife != null)
        {
            playerLife.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}