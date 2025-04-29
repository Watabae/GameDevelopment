using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 2f;

    private Vector2 direction;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        Destroy(gameObject, lifeTime); // Destrói depois de um tempo
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject); // Destroi o inimigo
            Destroy(gameObject); // Destroi a bala
        }
        else if (!other.CompareTag("Player"))
        {
            Destroy(gameObject); // Destroi a bala ao bater em qualquer coisa que não seja o player
        }
    }
}


