using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public float speed = 3f; // Velocidade do asteroide
    public int health = 1; // Vida do asteroide (pode ser aumentado para asteroides mais resistentes)
    public GameObject explosionPrefab; // Prefab com a animação de explosão
    public float explosionDuration = 1f; // Tempo que a explosão ficará visível
    private Rigidbody2D rb;
    // Variável para o multiplicador de velocidade
    private static float speedMultiplier = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Define o movimento da direita para a esquerda
        rb.velocity = Vector2.left * speed * speedMultiplier;
    }

    void Update()
    {
        // Atualiza a velocidade com o multiplicador atual
        rb.velocity = Vector2.left * speed * speedMultiplier;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica se colidiu com um projétil
        if (collision.CompareTag("Projectile"))
        {
            Destroy(collision.gameObject); // Destroi o projétil
            TakeDamage(1); // Reduz a vida do asteroide
        }
    }

    void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            TriggerExplosion(); // Dispara o efeito de explosão antes de destruir o asteroide
            if (gameObject.CompareTag("BigAsteroid"))
            {
                GameManager.instance.AddScore(300); // Pontuação para asteroides grandes
            }
            else if (gameObject.CompareTag("SmallAsteroid"))
            {
                GameManager.instance.AddScore(100); // Pontuação para asteroides pequenos
            }
            Destroy(gameObject); // Destroi o asteroide
        }
    }

    void TriggerExplosion()
    {
        if (explosionPrefab != null)
        {
            // Instancia o efeito de explosão com animação na posição do asteroide
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            // Destroi o efeito de explosão após um tempo (explosionDuration)
            Destroy(explosion, explosionDuration);
        }
    }

    // Método para alterar dinamicamente a velocidade dos asteroides
    public static void SetAsteroidSpeedMultiplier(float multiplier)
    {
        speedMultiplier = multiplier;
    }
}