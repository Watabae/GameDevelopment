using UnityEngine;

public class SpaceshipController : MonoBehaviour
{
    public float speed = 10f;
    [Header("Limites da Área de Movimento")]
    public float minY = -4.5f;
    public float maxY = 4.5f;
    [Header("Configuração do Tiro")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 15f;
    public float fireRate = 0.1f;
    private float nextFireTime = 0f;
    public SpriteRenderer spriteRenderer;

    void Start()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    void Update()
    {
        float moveY = Input.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3(0f, moveY, 0f) * speed * Time.deltaTime;
        transform.position += moveDirection;

        float clampedY = Mathf.Clamp(transform.position.y, minY, maxY);
        transform.position = new Vector3(transform.position.x, clampedY, transform.position.z);

        // Modificação aqui: usar Input.GetKey(KeyCode.Space) para atirar
        if (Input.GetKey(KeyCode.Space) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.right * projectileSpeed;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BigAsteroid") || collision.CompareTag("SmallAsteroid"))
        {
            Debug.Log("🚀 O jogador foi atingido! Fim do jogo.");
            GameManager.instance.GameOver();
            Time.timeScale = 0;
        }
    }

    public void SetSprite(Sprite newSprite)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = newSprite;
        }
        else
        {
            Debug.LogError("SpriteRenderer não encontrado na nave!");
        }
    }
}