using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Transform[] waypoints;
    public float moveSpeed = 2f;

    private int currentWaypointIndex = 0;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        MoveToWaypoint();
    }

    void MoveToWaypoint()
    {
        if (waypoints.Length == 0) return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];

        // SOMENTE o X muda, o Y é fixado
        Vector2 newPosition = new Vector2(
            Mathf.MoveTowards(transform.position.x, targetWaypoint.position.x, moveSpeed * Time.deltaTime),
            transform.position.y
        );

        transform.position = newPosition;

        if (Mathf.Abs(transform.position.x - targetWaypoint.position.x) < 0.1f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            sr.flipX = !sr.flipX; // Vira o sprite
        }   
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 contactPoint = collision.contacts[0].point;
            Vector2 center = GetComponent<Collider2D>().bounds.center;

            bool hitFromAbove = contactPoint.y > center.y + 0.8f;

            if (hitFromAbove)
            {
                // Inimigo morre
                Destroy(gameObject);
            }
            else
            {
                // Player perde vida
                GameManager.instance.LoseLife();
            }
        }
    }
}