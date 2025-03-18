using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject projectilePrefab; // Prefab do projétil
    public Transform firePoint; // Ponto de origem do tiro
    public float projectileSpeed = 10f;
    public float fireRate = 0.5f; // Tempo entre tiros (0.5 segundos)
    
    private float nextFireTime = 0f; // Armazena o tempo do próximo tiro permitido

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate; // Atualiza o tempo do próximo tiro
        }
    }

    void Shoot()
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.up * projectileSpeed; // Faz o projétil subir
        }
    }
}
