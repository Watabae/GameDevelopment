using System.Collections;
using UnityEngine;

public class MotherShipController : MonoBehaviour
{
    public float speed = 5f; // Velocidade da nave-mãe
    public GameObject explosionEffect; // Efeito de explosão
    public float explosionLifetime = 1.0f; // Tempo para a explosão desaparecer
    private float leftLimit, rightLimit;
    private bool movingRight = true; // Alternar entre direções
    private bool isWaiting = false; // Flag para evitar múltiplas coroutines

    void Start()
    {
        // Calcula os limites de tela
        float screenHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;
        leftLimit = -screenHalfWidth - 1f;  // Um pouco além da borda esquerda
        rightLimit = screenHalfWidth + 1f;  // Um pouco além da borda direita
        // Define a posição inicial
        transform.position = new Vector3(leftLimit, transform.position.y, transform.position.z);
        movingRight = true;
    }

    void Update()
    {
        // Movimentação contínua
        if (!isWaiting)
        {
            if (movingRight)
            {
                transform.position += Vector3.right * speed * Time.deltaTime;
                if (transform.position.x >= rightLimit)
                {
                    StartCoroutine(ChangeDirection());
                }
            }
            else
            {
                transform.position += Vector3.left * speed * Time.deltaTime;
                if (transform.position.x <= leftLimit)
                {
                    StartCoroutine(ChangeDirection());
                }
            }
        }
    }

    IEnumerator ChangeDirection()
    {
        isWaiting = true;
        yield return new WaitForSeconds(10f); // Aguarda 10 segundos
        movingRight = !movingRight; // Alterna a direção
        isWaiting = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Projectile"))
        {
            Destroy(other.gameObject); // Destroi o projétil
            Explode();
        }
    }

    void Explode()
    {
        if (explosionEffect != null)
        {
            GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(explosion, explosionLifetime); // Destroi a explosão após um tempo
        }
        
        GameManager.AddScore(1000);
        Destroy(gameObject); // Destroi a nave-mãe permanentemente
    }
}