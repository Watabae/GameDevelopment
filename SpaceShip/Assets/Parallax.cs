using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float length;
    public float parallaxEffect;
    private static float speedMultiplier = 1f; // Multiplicador da velocidade

    // Novas variáveis para controlar o spawn de objetos no fundo
    public GameObject backgroundObjectPrefab; // Prefab do objeto que se move no fundo (ex: estrela cadente)
    public float spawnInterval = 5f; // Intervalo de tempo entre os spawns
    public float spawnRangeY = 4f; // Faixa de altura para spawn aleatório
    public float objectSpeed = 2f; // Velocidade do objeto em movimento

    private float timeSinceLastSpawn = 0f;

    void Start()
    {
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        // Movimento do parallax no fundo
        transform.position += Vector3.left * Time.deltaTime * parallaxEffect * speedMultiplier;

        if (transform.position.x < -length)
        {
            transform.position = new Vector3(length, transform.position.y, transform.position.z);
        }

        // Controle para o spawn aleatório dos objetos no fundo
        timeSinceLastSpawn += Time.deltaTime;
        if (timeSinceLastSpawn >= spawnInterval)
        {
            SpawnBackgroundObject();
            timeSinceLastSpawn = 0f; // Resetando o timer
        }
    }

    // Método para spawnar objetos no fundo
    void SpawnBackgroundObject()
    {
        // Spawn aleatório de objetos dentro do range Y
        float spawnY = Random.Range(-spawnRangeY, spawnRangeY);
        Vector3 spawnPosition = new Vector3(length, spawnY, 0f); // Começa a partir da borda direita

        GameObject newBackgroundObject = Instantiate(backgroundObjectPrefab, spawnPosition, Quaternion.identity);
        BackgroundObjectMover mover = newBackgroundObject.AddComponent<BackgroundObjectMover>();
        mover.SetSpeed(objectSpeed); // Definindo a velocidade do objeto

        // O objeto se moverá para a esquerda, no efeito Parallax
        mover.StartMoving(parallaxEffect, speedMultiplier);
    }

    // Método para alterar dinamicamente a velocidade do parallax
    public static void SetParallaxSpeedMultiplier(float multiplier)
    {
        speedMultiplier = multiplier;
    }
}
