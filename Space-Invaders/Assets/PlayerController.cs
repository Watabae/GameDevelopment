using UnityEngine;
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb2d;
    public float speed = 5f;
    public float boundary = 8f; // Limite da tela
    public GameObject explosionEffect;
    public float explosionLifetime = 1.0f;
    
    private int lives = 3; // Total de vidas do jogador
    void Update()
    {
        float move = 0f;
        if (Input.GetKey(KeyCode.A))
        {
            move = -speed;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            move = speed;
        }
        transform.position += Vector3.right * move * Time.deltaTime;
        
        // Mantém a nave dentro dos limites da tela
        float clampedX = Mathf.Clamp(transform.position.x, -boundary, boundary);
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnemyProjectile"))
        {
            Destroy(other.gameObject);
            TakeDamage();
        }
    }
    public void TakeDamage()
    {
        GameManager.LoseLife();
        if (GameManager.PlayerLives <= 0)
        {
            Explode();
        }
    }
    void Explode()
    {
        if (explosionEffect != null)
        {
            GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(explosion, explosionLifetime);
        }
        GameManager.SetGameOver();
        Destroy(gameObject);
    }
    
    public void KillPlayer()
    {
        GameManager.PlayerLives = 0;
        Explode();
    }
}