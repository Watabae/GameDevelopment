using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
public class EnemyController : MonoBehaviour
{
    private Rigidbody2D rb2d;
    private float timer = 0.0f;
    private float waitTime = 3.0f; // Tempo entre mudanças de direção
    private float speed = 1.0f;
    private bool ShouldGoDown = false;
    public GameObject explosionEffect;
    public float explosionLifetime = 1.0f;
    public GameObject projectilePrefab; // Prefab do projétil do inimigo
    public float projectileSpeed = 5f; // Velocidade do projétil do inimigo
    private float shootTimer = 0f; // Timer para disparo
    private float minShootInterval = 2f; // Intervalo mínimo entre disparos
    private float maxShootInterval = 5f; // Intervalo máximo entre disparos
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        var vel = rb2d.velocity;
        vel.x = speed;
        rb2d.velocity = vel;
        // Inicia o timer para disparo aleatório
        shootTimer = Random.Range(minShootInterval, maxShootInterval);
    }
    void Update()
    {
        // Limite de posição
        if (transform.position.x > Camera.main.orthographicSize * Camera.main.aspect)
            transform.position = new Vector3(Camera.main.orthographicSize * Camera.main.aspect, transform.position.y, transform.position.z);
        else if (transform.position.x < -Camera.main.orthographicSize * Camera.main.aspect)
            transform.position = new Vector3(-Camera.main.orthographicSize * Camera.main.aspect, transform.position.y, transform.position.z);
        timer += Time.deltaTime;
        if (timer >= waitTime)
        {
            ChangeState();
            timer = 0.0f;
        }
        // Disparo aleatório
        if (gameObject.tag == "Shooter") // Verifica se o inimigo tem a tag "Shooter"
        {
        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0f)
        {
            Shoot();
            shootTimer = Random.Range(minShootInterval, maxShootInterval); // Reinicia o timer
        }
        }
        if (transform.position.y <= -5.6f)
        {
            if (gameObject.CompareTag("Bomber") || gameObject.CompareTag("Shooter") || gameObject.CompareTag("Tank"))
            {
                KillPlayer();
            }
            Destroy(gameObject);
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            KillPlayer();
            Destroy(gameObject);
        }
    }
    public void KillPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.KillPlayer();
            }
        }
    }
    
    void ChangeState()
    {
        var vel = rb2d.velocity;
        vel.x *= -1;
        rb2d.velocity = vel;
        if (ShouldGoDown)
        {
            rb2d.position = new Vector2(rb2d.position.x, rb2d.position.y - 1);
        }
        ShouldGoDown = !ShouldGoDown;
    }
    void Shoot()
    {
        // Cria o projétil
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.down * projectileSpeed; // Faz o projétil descer
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Projectile"))
        {
            Explode();
            Destroy(other.gameObject);
        }
    }
    void Explode()
    {
        if (explosionEffect != null)
        {
            GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Rigidbody2D explosionRb = explosion.GetComponent<Rigidbody2D>();
            if (explosionRb != null)
            {
                explosionRb.velocity = rb2d.velocity;
            }
            Destroy(explosion, explosionLifetime);
        }

        if (gameObject.tag == "Tank")
        {
            GameManager.AddScore(100);
        }
        else if (gameObject.tag == "Bomber")
        {
            LaunchBomberProjectiles();
            GameManager.AddScore(300);
        }
        else if (gameObject.tag == "Shooter")
        {
            GameManager.AddScore(500);
        }
        Destroy(gameObject); // Destroi o inimigo após a explosão
    }
    void LaunchBomberProjectiles()
    {
        // Cria múltiplos projéteis para simular uma explosão
        for (int i = 0; i < 5; i++) // Ajuste a quantidade de projéteis conforme necessário
        {
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Ajusta a velocidade e direção dos projéteis para que eles desçam
                rb.velocity = Vector2.down * projectileSpeed;
            }
        }
    }
}