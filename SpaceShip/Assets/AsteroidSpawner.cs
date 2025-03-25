using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs; // Array de enemyes
    public float[] spawnRates; // Taxas de spawn para cada tipo de enemye
    public float spawnX = -10f; // Posição X fixa (parede esquerda)
    public float minY = -5f, maxY = 5f; // Define a altura do spawn

    private float[] nextSpawnTimes; // Array para controlar o tempo de spawn de cada enemye

    void Start()
    {
        nextSpawnTimes = new float[enemyPrefabs.Length]; // Inicializa o array de tempos de spawn

        // Inicializa os tempos de spawn
        for (int i = 0; i < enemyPrefabs.Length; i++)
        {
            nextSpawnTimes[i] = Time.time + Random.Range(0f, spawnRates[i]); // Spawna com um intervalo aleatório
        }
    }

    void Update()
    {
        for (int i = 0; i < enemyPrefabs.Length; i++)
        {
            // Verifica se é hora de spawnar o enemye i
            if (Time.time >= nextSpawnTimes[i])
            {
                Spawnenemy(i); // Spawn do enemye
                nextSpawnTimes[i] = Time.time + spawnRates[i]; // Atualiza o tempo de spawn para o próximo
            }
        }
    }

    void Spawnenemy(int index)
    {
        float spawnY = Random.Range(minY, maxY);
        Vector2 spawnPosition = new Vector2(spawnX, spawnY);

        GameObject enemy = Instantiate(enemyPrefabs[index], spawnPosition, Quaternion.identity);
        Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.velocity = new Vector2(Mathf.Abs(3f), 0f); // Define a velocidade do enemye
            rb.angularVelocity = 0f; // Impede rotação
        }

        enemy.transform.rotation = Quaternion.identity; // Impede rotação ao spawn
    }
}
