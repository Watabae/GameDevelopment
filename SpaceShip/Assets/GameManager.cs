using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Necessário para carregar a cena
using System.Collections; // Necessário para usar IEnumerator e StartCoroutine

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Text scoreText;
    public Font customFont;

    public GameObject gameOverUI; // Referência para o painel de Game Over
    public Text gameOverText; // Texto de Game Over
    public Button restartButton; // Botão de reiniciar

    private int score = 0;
    private bool isSlowMotionActive = false;

    private float slowMotionFactor = 0.5f; // Redução de velocidade (50%)
    private float slowMotionDuration = 5f; // Tempo do efeito em segundos

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        UpdateScoreText();
        if (customFont != null)
        {
            scoreText.font = customFont;
        }

        // Inicializa a UI de Game Over e desativa
        gameOverUI.SetActive(false);

        // Adiciona a função de reiniciar ao botão
        restartButton.onClick.AddListener(RestartGame);
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateScoreText();

        // Ativa câmera lenta se o score for múltiplo de 2000
        if (score % 2000 == 0 && !isSlowMotionActive)
        {
            StartCoroutine(SlowMotionEffect());
        }
    }

    void UpdateScoreText()
    {
        scoreText.text = "Score: " + score;
    }

    IEnumerator SlowMotionEffect()
    {
        isSlowMotionActive = true;

        // Reduz a velocidade dos asteroides e do background
        Asteroid.SetAsteroidSpeedMultiplier(slowMotionFactor);
        Parallax.SetParallaxSpeedMultiplier(slowMotionFactor);

        yield return new WaitForSeconds(slowMotionDuration);

        // Retorna à velocidade normal
        Asteroid.SetAsteroidSpeedMultiplier(1f);
        Parallax.SetParallaxSpeedMultiplier(1f);

        isSlowMotionActive = false;
    }

    public void GameOver()
    {
        Time.timeScale = 0; // Pausa o jogo

        // Ativa a UI de Game Over
        gameOverUI.SetActive(true);
    }

    void RestartGame()
    {
        // Desativa a UI de Game Over
        gameOverUI.SetActive(false);

        // Reinicia a cena atual
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1; // Retorna o tempo para o normal
    }
}
