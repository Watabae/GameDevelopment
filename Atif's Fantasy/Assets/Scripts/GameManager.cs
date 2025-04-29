using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Adicione isso!

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UI")]
    public Text coinText;
    public Text livesText;
    public Image coinIcon;
    public Image lifeIcon;

    [Header("Game Data")]
    public int coinCount = 0;
    public int lives = 3;

    [Header("Scene Names")]
    public string gameOverSceneName = "GameOver"; // Nome da cena de Game Over

    void Awake()
    {
        // Singleton
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        UpdateUI();
    }

    public void AddCoin()
    {
        coinCount++;
        UpdateUI();
    }

    public void LoseLife()
    {
        lives--;
        UpdateUI();

        if (lives <= 0)
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        Debug.Log("Game Over");
        SceneManager.LoadScene(gameOverSceneName);
    }

    void UpdateUI()
    {
        if (coinText != null)
            coinText.text = coinCount.ToString();

        if (livesText != null)
            livesText.text = lives.ToString();
    }
}
