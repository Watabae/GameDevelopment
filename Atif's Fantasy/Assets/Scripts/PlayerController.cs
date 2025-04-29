using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    public GameObject bulletPrefab;
    public Transform firePoint;


    private Rigidbody2D rb;
    private Animator anim;
    private Vector3 startPosition;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        startPosition = transform.position;
    }

    void Update()
    {
        if (isDead) return; // Impede controle enquanto está "morrendo"

        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        if (moveInput != 0)
            transform.localScale = new Vector3(Mathf.Sign(moveInput), 1, 1);

        if (Input.GetKeyDown(KeyCode.Space) && Mathf.Abs(rb.velocity.y) < 0.01f)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            Shoot();
        }

        anim.SetFloat("Speed", Mathf.Abs(moveInput));
        anim.SetBool("isJumping", Mathf.Abs(rb.velocity.y) > 0.01f);
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        bullet.GetComponent<Bullet>().SetDirection(direction);

        anim.SetTrigger("Shoot");
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Trap"))
        {
            TakeDamage();
        }
    }

    void TakeDamage()
    {
        if (isDead) return; // Evita múltiplas chamadas

        isDead = true;
        anim.SetTrigger("Die");
        rb.velocity = Vector2.zero;

        GameManager.instance.LoseLife();

        if (GameManager.instance.lives > 0)
        {
            Invoke("Respawn", 1.2f); // Espera a animação terminar
        }
        else
        {
            Invoke("GameOver", 1.2f);
        }
    }

    void Respawn()
    {
        transform.position = startPosition;
        isDead = false;
    }

    void GameOver()
    {
        gameObject.SetActive(false);
        GameManager.instance.GameOver();
    }
}
