using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static int PlayerScore = 0;
    public static int PlayerLives = 3;
    public static bool IsGameOver = false;
    public static bool IsGameWon = false;
    public Sprite lifeSprite;
    public Font customFont;
    private GUIStyle scoreStyle;
    private GUIStyle livesTextStyle;
    private GUIStyle gameOverStyle;
    private GUIStyle victoryStyle;
    private GUIStyle messageStyle;

    void Start()
    {
        InitializeStyles();
    }

    void InitializeStyles()
    {
        scoreStyle = new GUIStyle();
        scoreStyle.font = customFont;
        scoreStyle.fontSize = 48;
        scoreStyle.normal.textColor = Color.black;
        scoreStyle.alignment = TextAnchor.MiddleCenter;

        livesTextStyle = new GUIStyle(scoreStyle);

        gameOverStyle = new GUIStyle(scoreStyle);
        gameOverStyle.fontSize = 72;
        gameOverStyle.normal.textColor = Color.red;

        victoryStyle = new GUIStyle(scoreStyle);
        victoryStyle.fontSize = 72;
        victoryStyle.normal.textColor = Color.green;

        messageStyle = new GUIStyle(scoreStyle);
        messageStyle.fontSize = 36;
        messageStyle.normal.textColor = Color.white;
    }

    void Update()
    {
        if (!IsGameOver && !IsGameWon)
        {
            CheckVictoryCondition();
        }
        else if ((IsGameOver || IsGameWon) && Input.GetKeyDown(KeyCode.Space))
        {
            if (IsGameOver)
            {
                RestartGame();
            }
            else if (IsGameWon)
            {
                ContinueGame();
            }
        }
    }

    void CheckVictoryCondition()
    {
        if (!IsGameOver && !IsGameWon)
        {
            bool allEnemiesDefeated = GameObject.FindGameObjectsWithTag("Bomber").Length == 0 &&
                                      GameObject.FindGameObjectsWithTag("Tank").Length == 0 &&
                                      GameObject.FindGameObjectsWithTag("Shooter").Length == 0;

            if (allEnemiesDefeated)
            {
                SetGameWon();
            }
        }
    }

    public static void AddScore(int points)
    {
        PlayerScore += points;
    }

    public static void LoseLife()
    {
        if (PlayerLives > 0)
        {
            PlayerLives--;
            if (PlayerLives <= 0)
            {
                SetGameOver();
            }
        }
    }

    public static void GainLife()
    {
        if (PlayerLives < 3)
        {
            PlayerLives++;
        }
    }

    public static void SetGameOver()
    {
        IsGameOver = true;
        Time.timeScale = 0;
    }

    public static void SetGameWon()
    {
        IsGameWon = true;
        Time.timeScale = 0;
    }

    void OnGUI()
    {
        // Desenha o score
        Rect scoreRect = new Rect(Screen.width / 2 + 260, 450, 200, 100);
        GUI.Label(scoreRect, "SCORE: " + PlayerScore, scoreStyle);

        // Desenha o texto "LIVES:" acima dos ícones de vida
        Rect livesTextRect = new Rect(75, 450, 130, 30);
        GUI.Label(livesTextRect, "LIVES:", livesTextStyle);

        // Desenha os ícones de vida
        DrawPlayerLives();

        if (IsGameOver)
        {
            DrawGameOverScreen();
        }
        else if (IsGameWon)
        {
            DrawVictoryScreen();
        }
    }

    private void DrawPlayerLives()
    {
        if (lifeSprite == null)
        {
            Debug.LogError("Life sprite is not assigned!");
            return;
        }
        float lifeX = 83;
        float lifeY = 500;
        float lifeSpacing = 10;
        float spriteSize = 30;
        for (int i = 0; i < PlayerLives; i++)
        {
            Rect spriteRect = new Rect(lifeX + (lifeSpacing + spriteSize) * i, lifeY, spriteSize, spriteSize);
            GUI.DrawTexture(spriteRect, lifeSprite.texture, ScaleMode.ScaleToFit);
        }
    }

    private void DrawGameOverScreen()
    {
        // Desenha um fundo semi-transparente
        GUI.color = new Color(0, 0, 0, 0.75f);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        GUI.color = Color.white;

        // Desenha o texto "GAME OVER" em vermelho
        Rect gameOverRect = new Rect(Screen.width / 2 - 200, Screen.height / 2 - 50, 400, 100);
        GUI.Label(gameOverRect, "GAME OVER", gameOverStyle);

        // Desenha a mensagem "Press Space to restart"
        Rect messageRect = new Rect(Screen.width / 2 - 200, Screen.height / 2 + 50, 400, 50);
        GUI.Label(messageRect, "Press Space to restart", messageStyle);
    }

    private void DrawVictoryScreen()
    {
        // Desenha um fundo semi-transparente
        GUI.color = new Color(0, 0, 0, 0.75f);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        GUI.color = Color.white;

        // Desenha o texto "VICTORY!" em verde
        Rect victoryRect = new Rect(Screen.width / 2 - 200, Screen.height / 2 - 50, 400, 100);
        GUI.Label(victoryRect, "VICTORY!", victoryStyle);

        // Desenha a mensagem "Press Space to continue"
        Rect messageRect = new Rect(Screen.width / 2 - 200, Screen.height / 2 + 50, 400, 50);
        GUI.Label(messageRect, "Press Space to continue", messageStyle);
    }

    private void RestartGame()
    {
        PlayerScore = 0;
        PlayerLives = 3;
        IsGameOver = false;
        IsGameWon = false;
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void ContinueGame()
    {
    PlayerLives = Mathf.Max(1, PlayerLives);
    IsGameWon = false;
    Time.timeScale = 1;
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}