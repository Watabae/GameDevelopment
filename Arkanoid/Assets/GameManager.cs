using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton para persistir entre cenas
    public static GameManager instance;
    
    public static int PlayerScore = 0;
    private bool gameOver = false;
    private bool gameCompleted = false; // Novo estado para quando o jogo inteiro for concluído
    private bool initialized = false;

    // Progressão de cenas
    public int pointsToNextLevel = 2800; // Pontos necessários para avançar para a próxima cena
    public int pointsToCompleteGame = 5800; // Pontos necessários para completar o jogo na fase 2
    private int currentSceneIndex; // Índice da cena atual
    private bool levelCompleted = false; // Indica se o nível foi completado
    public float nextLevelDelay = 3f; // Tempo de espera antes de avançar para o próximo nível

    // Sons
    public AudioClip loseLifeSound; // Som quando perde uma vida
    public AudioClip gameOverSound; // Som de game over
    public AudioClip levelCompletedSound; // Som quando completa o nível
    public AudioClip gameCompletedSound; // Som quando completa o jogo inteiro
    private AudioSource audioSource;
    
    // Sistema de vidas
    public int initialLives = 3;
    private int currentLives;
    public Texture2D lifeIcon; // Ícone que representa uma vida
    public Vector2 lifeIconSize = new Vector2(30, 30);
    public Vector2 firstLifePosition = new Vector2(20, 20);
    public float lifeIconSpacing = 10;
    
    // Controle de áudio
    private List<AudioSource> pausedAudioSources = new List<AudioSource>();
    public float gameOverSoundVolume = 1.0f;

    public GUISkin layout;
    public Font arcadeFont;
    public int scoreFontSize = 48;
    GameObject theBall;

    private List<BrickInfo> originalBricks = new List<BrickInfo>();
    public GameObject brickTemplate;
    private bool bricksStored = false;

    // Estrutura para guardar informações de cada bloco
    [System.Serializable]
    public class BrickInfo
    {
        public Vector3 position;
        public Vector3 scale;
        public Quaternion rotation;
        public AudioClip breakSound; // Armazenar o som de quebra
        
        public BrickInfo(Vector3 pos, Vector3 scl, Quaternion rot, AudioClip sound)
        {
            position = pos;
            scale = scl;
            rotation = rot;
            breakSound = sound;
        }
    }

    void Awake()
    {
        // Implementação do Singleton para manter apenas uma instância entre cenas
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Configura o AudioSource para os sons do sistema
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.volume = gameOverSoundVolume;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Inicializa o sistema na primeira execução
        if (!initialized)
        {
            currentLives = initialLives;
            initialized = true;
        }
        
        // Configura baseado na cena atual
        SetupCurrentScene();
        
        // Adiciona listener para mudanças de cena
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        // Remove o listener quando o objeto for destruído
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Configura o jogo para a cena atual
    private void SetupCurrentScene()
    {
        theBall = GameObject.FindGameObjectWithTag("Ball");
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        levelCompleted = false;
        gameCompleted = false;
        bricksStored = false;
        StartCoroutine(InitializeBricks());
        
        // Reajusta pontos necessários baseados na cena
        if (currentSceneIndex == 0) // Level 1
        {
            pointsToNextLevel = 2800;
        }
        else if (currentSceneIndex == 1) // Level 2
        {
            pointsToNextLevel = pointsToCompleteGame;
        }
    }

    // Chamado quando uma nova cena é carregada
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetupCurrentScene();
    }

    void Update()
    {
        // Não faz verificações se o jogo estiver acabado ou nível completo
        if (gameOver || levelCompleted || gameCompleted)
            return;
            
        // Verifica se está na fase 2 e atingiu pontos para completar o jogo
        if (currentSceneIndex == 1 && PlayerScore >= pointsToCompleteGame)
        {
            GameCompleted();
        }
        // Fase 1 - verifica pontos para avançar para próxima fase
        else if (currentSceneIndex == 0 && PlayerScore >= pointsToNextLevel)
        {
            LevelCompleted();
        }
    }

    // Método para quando o jogador completa o jogo inteiro
    private void GameCompleted()
    {
        gameCompleted = true;
        
        // Toca o som de jogo completado
        MuteOtherSounds();
        
        if (audioSource != null && gameCompletedSound != null)
        {
            audioSource.clip = gameCompletedSound;
            audioSource.Play();
        }
        else if (audioSource != null && levelCompletedSound != null)
        {
            // Se não tiver som específico, usa o som de nível completado
            audioSource.clip = levelCompletedSound;
            audioSource.Play();
        }
        
        // Desativa a bola
        if (theBall != null)
        {
            theBall.SetActive(false);
        }
    }

    // Método para quando o jogador completa o nível
    private void LevelCompleted()
    {
        levelCompleted = true;
        
        // Toca o som de nível completado
        MuteOtherSounds();
        
        if (audioSource != null && levelCompletedSound != null)
        {
            audioSource.clip = levelCompletedSound;
            audioSource.Play();
        }
        
        // Inicia a transição para o próximo nível
        StartCoroutine(LoadNextLevel());
    }
    
    // Coroutine para carregar o próximo nível
    private IEnumerator LoadNextLevel()
    {
        // Desativa a bola
        if (theBall != null)
        {
            theBall.SetActive(false);
        }
        
        // Espera alguns segundos antes de avançar
        yield return new WaitForSeconds(nextLevelDelay);
        
        // Restaura os sons
        RestoreSounds();
        
        // Carrega a próxima cena
        int nextSceneIndex = currentSceneIndex + 1;
        
        // Verifica se a próxima cena existe
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            // Se não houver próxima cena, considera o jogo completo
            Debug.Log("Jogo completo! Não há mais níveis.");
            GameCompleted(); // Mostra tela de jogo completo
        }
    }

    IEnumerator InitializeBricks()
    {
        yield return new WaitForEndOfFrame();

        if (!bricksStored)
        {
            StoreBrickPositions();
            bricksStored = true;
            Debug.Log($"Blocos iniciais armazenados: {originalBricks.Count}");
        }
    }

    private void StoreBrickPositions()
    {
        originalBricks.Clear();
        GameObject[] bricks = GameObject.FindGameObjectsWithTag("Brick");
        
        foreach (GameObject brick in bricks)
        {
            // Pula blocos que são filhos de outros blocos com tag "Brick"
            if (brick.transform.parent != null && brick.transform.parent.CompareTag("Brick"))
            {
                continue;
            }
            
            // Obter o componente Brick para acessar o breakSound
            Brick brickComponent = brick.GetComponent<Brick>();
            AudioClip sound = null;
            if (brickComponent != null) 
            {
                sound = brickComponent.breakSound;
            }
            
            // Armazenar informações do bloco
            BrickInfo info = new BrickInfo(
                brick.transform.position,
                brick.transform.localScale,
                brick.transform.rotation,
                sound
            );
            
            originalBricks.Add(info);
            Debug.Log($"Armazenando bloco na posição: {brick.transform.position}");
        }
    }

    public static void AddScore(int points)
    {
        PlayerScore += points;
    }

    void OnGUI()
    {
        // Configuração inicial do skin
        GUI.skin = layout;

        // Mostra as vidas do jogador com ícones
        DrawPlayerLives();

        // Cria um estilo personalizado para o texto de pontuação
        GUIStyle scoreStyle = new GUIStyle();
        scoreStyle.fontSize = scoreFontSize;
        scoreStyle.font = arcadeFont;
        scoreStyle.normal.textColor = Color.white;
        scoreStyle.alignment = TextAnchor.MiddleCenter; // Centraliza o texto
        
        // Adiciona sombreamento para melhorar a visibilidade
        scoreStyle.normal.background = null; // Remove qualquer fundo
        scoreStyle.fontStyle = FontStyle.Bold; // Deixa a fonte em negrito
        
        // Desenha uma sombra para o texto (melhora visibilidade)
        GUIStyle shadowStyle = new GUIStyle(scoreStyle);
        shadowStyle.normal.textColor = new Color(0, 0, 0, 0.75f); // Sombra preta semi-transparente
        
        // Posição do texto e dimensões
        Rect scoreRect = new Rect(Screen.width / 2 + 200, 50, 200, 100);
        
        // Primeiro desenha a sombra (ligeiramente deslocada)
        GUI.Label(new Rect(scoreRect.x + 2, scoreRect.y + 2, scoreRect.width, scoreRect.height), 
                "SCORE: " + PlayerScore, shadowStyle);
        
        // Depois desenha o texto principal
        GUI.Label(scoreRect, "SCORE: " + PlayerScore, scoreStyle);

        // Mostra a meta de pontos para o próximo nível 
        if (!gameOver && !levelCompleted && !gameCompleted)
        {
            GUIStyle goalStyle = new GUIStyle(scoreStyle);
            goalStyle.fontSize = 24;
            
            Rect goalRect = new Rect(Screen.width / 2 + 200, 110, 200, 50);
            string goalText;
            
            if (currentSceneIndex == 0)
                goalText = $"GOAL: {PlayerScore}/{pointsToNextLevel}";
            else
                goalText = $"GOAL: {PlayerScore}/{pointsToCompleteGame}";
            
            GUI.Label(new Rect(goalRect.x + 2, goalRect.y + 2, goalRect.width, goalRect.height), 
                    goalText, shadowStyle);
            GUI.Label(goalRect, goalText, goalStyle);
        }

        // Se o jogo foi completado, mostra a mensagem de "GAME COMPLETED"
        if (gameCompleted)
        {
            // Cria um background semi-transparente
            GUI.DrawTexture(
                new Rect(0, 0, Screen.width, Screen.height),
                MakeTexture(2, 2, new Color(0, 0, 0, 0.5f))
            );
            
            // Mostra a mensagem de GAME COMPLETED
            GUIStyle completedStyle = new GUIStyle(scoreStyle);
            completedStyle.fontSize = 70;
            completedStyle.normal.textColor = new Color(1f, 0.8f, 0.2f); // Dourado
            
            Rect completedRect = new Rect(Screen.width / 2 - 300, Screen.height / 2 - 150, 600, 120);
            
            // Sombra para o texto
            GUIStyle completedShadowStyle = new GUIStyle(completedStyle);
            completedShadowStyle.normal.textColor = new Color(0.5f, 0.3f, 0, 0.75f); // Sombra dourada escura
            
            GUI.Label(new Rect(completedRect.x + 4, completedRect.y + 4, completedRect.width, completedRect.height),
                      "GAME COMPLETED!", completedShadowStyle);
            GUI.Label(completedRect, "GAME COMPLETED!", completedStyle);
            
            // Mostra a pontuação final
            GUIStyle finalScoreStyle = new GUIStyle(scoreStyle);
            finalScoreStyle.fontSize = 40;
            finalScoreStyle.normal.textColor = Color.white;
            
            Rect finalScoreRect = new Rect(Screen.width / 2 - 200, Screen.height / 2, 400, 80);
            string finalScoreText = $"FINAL SCORE: {PlayerScore}";
            
            GUI.Label(new Rect(finalScoreRect.x + 3, finalScoreRect.y + 3, finalScoreRect.width, finalScoreRect.height),
                      finalScoreText, shadowStyle);
            GUI.Label(finalScoreRect, finalScoreText, finalScoreStyle);
            
            // Botão para reiniciar o jogo
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = 24;
            buttonStyle.font = arcadeFont;
            buttonStyle.normal.textColor = Color.white;
            buttonStyle.fontStyle = FontStyle.Bold;
            
            // Adiciona uma borda ao botão para melhorar a visibilidade
            buttonStyle.border = new RectOffset(5, 5, 5, 5);
            
            // Posição e dimensões do botão
            Rect buttonRect = new Rect(Screen.width / 2 - 150, Screen.height / 2 + 120, 300, 50);
            
            // Desenha um fundo semi-transparente para o botão
            GUI.DrawTexture(buttonRect, MakeTexture(2, 2, new Color(0.3f, 0.3f, 0.3f, 0.8f)));
            
            if (GUI.Button(buttonRect, "PLAY AGAIN", buttonStyle))
            {
                RestartGame();
            }
        }
        // Se o nível foi completado, mostra mensagem
        else if (levelCompleted)
        {
            // Cria um background semi-transparente
            GUI.DrawTexture(
                new Rect(0, 0, Screen.width, Screen.height),
                MakeTexture(2, 2, new Color(0, 0, 0, 0.4f))
            );
            
            // Mostra a mensagem de LEVEL COMPLETE
            GUIStyle completeStyle = new GUIStyle(scoreStyle);
            completeStyle.fontSize = 60;
            completeStyle.normal.textColor = Color.green;
            
            Rect completeRect = new Rect(Screen.width / 2 - 250, Screen.height / 2 - 100, 500, 100);
            
            // Sombra para o texto
            GUIStyle completeShadowStyle = new GUIStyle(completeStyle);
            completeShadowStyle.normal.textColor = new Color(0, 0, 0, 0.75f);
            
            GUI.Label(new Rect(completeRect.x + 3, completeRect.y + 3, completeRect.width, completeRect.height),
                      "LEVEL COMPLETE!", completeShadowStyle);
            GUI.Label(completeRect, "LEVEL COMPLETE!", completeStyle);
            
            // Mensagem de carregando o próximo nível
            GUIStyle loadingStyle = new GUIStyle(scoreStyle);
            loadingStyle.fontSize = 24;
            loadingStyle.normal.textColor = Color.white;
            
            Rect loadingRect = new Rect(Screen.width / 2 - 150, Screen.height / 2 + 30, 300, 50);
            GUI.Label(new Rect(loadingRect.x + 2, loadingRect.y + 2, loadingRect.width, loadingRect.height),
                      "LOADING NEXT LEVEL...", shadowStyle);
            GUI.Label(loadingRect, "LOADING NEXT LEVEL...", loadingStyle);
        }
        // Lógica para o Game Over (quando não há mais vidas)
        else if (gameOver)
        {
            // Cria um background semi-transparente para a tela de game over
            GUI.DrawTexture(
                new Rect(0, 0, Screen.width, Screen.height),
                MakeTexture(2, 2, new Color(0, 0, 0, 0.6f))
            );
            
            // Mostra a mensagem de GAME OVER
            GUIStyle gameOverStyle = new GUIStyle(scoreStyle);
            gameOverStyle.fontSize = 60;
            gameOverStyle.normal.textColor = Color.red;
            
            Rect gameOverRect = new Rect(Screen.width / 2 - 200, Screen.height / 2 - 100, 400, 100);
            
            // Sombra para o texto GAME OVER
            GUIStyle gameOverShadowStyle = new GUIStyle(gameOverStyle);
            gameOverShadowStyle.normal.textColor = new Color(0, 0, 0, 0.75f);
            
            GUI.Label(new Rect(gameOverRect.x + 3, gameOverRect.y + 3, gameOverRect.width, gameOverRect.height),
                      "GAME OVER", gameOverShadowStyle);
            GUI.Label(gameOverRect, "GAME OVER", gameOverStyle);

            // Estilo personalizado para o botão
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = 20;
            buttonStyle.font = arcadeFont;
            buttonStyle.normal.textColor = Color.white;
            buttonStyle.fontStyle = FontStyle.Bold;
            
            // Adiciona uma borda ao botão para melhorar a visibilidade
            buttonStyle.border = new RectOffset(5, 5, 5, 5);
            
            // Posição e dimensões do botão
            Rect buttonRect = new Rect(Screen.width / 2 - 100, Screen.height / 2 + 50, 200, 40);
            
            // Desenha um fundo semi-transparente para o botão
            GUI.DrawTexture(buttonRect, MakeTexture(2, 2, new Color(0.3f, 0.3f, 0.3f, 0.8f)));
            
            if (GUI.Button(buttonRect, "RESTART", buttonStyle))
            {
                RestartGame();
            }
        }
    }

    // Método para desenhar as vidas do jogador
    private void DrawPlayerLives()
    {
        if (lifeIcon == null)
        {
            // Se não houver ícone, desenha círculos simples
            for (int i = 0; i < currentLives; i++)
            {
                float x = firstLifePosition.x + (lifeIconSize.x + lifeIconSpacing) * i;
                float y = firstLifePosition.y;
                
                GUI.DrawTexture(
                    new Rect(x, y, lifeIconSize.x, lifeIconSize.y),
                    MakeTexture(2, 2, Color.red)
                );
            }
        }
        else
        {
            // Desenha o ícone para cada vida
            for (int i = 0; i < currentLives; i++)
            {
                float x = firstLifePosition.x + (lifeIconSize.x + lifeIconSpacing) * i;
                float y = firstLifePosition.y;
                
                GUI.DrawTexture(
                    new Rect(x, y, lifeIconSize.x, lifeIconSize.y),
                    lifeIcon
                );
            }
        }
        
        // Também mostra o número de vidas em texto (opcional)
        GUIStyle livesTextStyle = new GUIStyle();
        livesTextStyle.fontSize = 20;
        livesTextStyle.font = arcadeFont;
        livesTextStyle.normal.textColor = Color.white;
        livesTextStyle.fontStyle = FontStyle.Bold;
        
        float textX = firstLifePosition.x + (lifeIconSize.x + lifeIconSpacing) * initialLives + 10;
        GUI.Label(new Rect(textX, firstLifePosition.y, 100, lifeIconSize.y), 
                  "x " + currentLives, livesTextStyle);
    }

    // Método auxiliar para criar uma textura simples
    private Texture2D MakeTexture(int width, int height, Color color)
    {
        Color[] pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }
        
        Texture2D texture = new Texture2D(width, height);
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }

    // Método para perder uma vida
    public void LoseLife()
    {
        // Se o nível já foi completado ou o jogo foi completado, não faz nada
        if (levelCompleted || gameCompleted)
            return;
            
        currentLives--;
        
        // Toca o som de perda de vida
        if (audioSource != null && loseLifeSound != null)
        {
            audioSource.clip = loseLifeSound;
            audioSource.Play();
            audioSource.volume = 0.25f;
        }
        
        if (currentLives <= 0)
        {
            // Sem mais vidas, ativa o game over
            SetGameOver();
        }
        else
        {
            // Ainda há vidas, reseta a bola automaticamente
            StartCoroutine(AutoResetBall());
        }
    }
    
    // Coroutine para esperar um momento e então resetar a bola
    private IEnumerator AutoResetBall()
    {
        // Espera um curto momento para dar tempo do jogador ver que perdeu uma vida
        yield return new WaitForSeconds(1.0f);
        
        // Reinicia apenas a bola, não os blocos
        if (theBall != null)
        {
            theBall.SetActive(true);
            theBall.SendMessage("RestartGame", null, SendMessageOptions.RequireReceiver);
        }
    }

    // Método para mutar todos os outros sons da cena exceto o som atual
    private void MuteOtherSounds()
    {
        // Encontra todos os AudioSources na cena
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        
        pausedAudioSources.Clear();
        
        foreach (AudioSource source in allAudioSources)
        {
            // Ignora o próprio AudioSource do GameManager
            if (source != audioSource)
            {
                // Se o áudio estiver tocando, pausa-o e adiciona à lista
                if (source.isPlaying)
                {
                    source.Pause();
                    pausedAudioSources.Add(source);
                }
            }
        }
        
        // Define nosso AudioSource para tocar o som em volume alto
        if (audioSource != null)
        {
            audioSource.volume = gameOverSoundVolume;
        }
    }
    
    // Método para restaurar os sons após o game over terminar
    private void RestoreSounds()
    {
        // Retoma a reprodução de áudios pausados
        foreach (AudioSource source in pausedAudioSources)
        {
            if (source != null) // Verifica se o objeto ainda existe
            {
                source.UnPause();
            }
        }
        
        pausedAudioSources.Clear();
    }

    // Método para ativar o estado de Game Over
    private void SetGameOver()
    {
        gameOver = true;
        
        // Muta outros sons antes de tocar o som de game over
        MuteOtherSounds();
        
        // Toca o som de game over
        if (audioSource != null && gameOverSound != null)
        {
            audioSource.clip = gameOverSound;
            audioSource.Play();
            
            // Inicia uma coroutine para restaurar os sons quando o som de game over terminar
            StartCoroutine(RestoreSoundsAfterDelay(6.0f)); // Tempo aproximado
        }
        
        if (theBall != null)
        {
            theBall.SetActive(false);
        }
    }
    
    // Coroutine para restaurar os sons após um delay
    private IEnumerator RestoreSoundsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        // Restaura os sons
        RestoreSounds();
    }

    // Para compatibilidade com scripts existentes
    public void GameOver()
    {
        LoseLife();
    }

    // Reinicia o jogo completamente (usado pelo botão RESTART ou PLAY AGAIN)
    public void RestartGame()
    {
        // Restaura os sons antes de reiniciar
        RestoreSounds();
        
        // Sempre volta para a cena 1 (índice 0) quando pressiona RESTART
        SceneManager.LoadScene(0);
        
        // Reseta a pontuação global
        PlayerScore = 0;
        
        // Reseta as vidas e estados
        currentLives = initialLives;
        gameOver = false;
        levelCompleted = false;
        gameCompleted = false;
    }

    private IEnumerator RecreateBricks()
    {
        // 1. Destruir todos os blocos existentes de maneira segura
        GameObject[] existingBricks = GameObject.FindGameObjectsWithTag("Brick");
        List<GameObject> rootBricks = new List<GameObject>();
        
        foreach (GameObject brick in existingBricks)
        {
            if (brick.transform.parent == null || !brick.transform.parent.CompareTag("Brick"))
            {
                rootBricks.Add(brick);
            }
        }
        
        Debug.Log($"Destruindo {rootBricks.Count} blocos raiz");
        
        foreach (GameObject brick in rootBricks)
        {
            Destroy(brick);
        }
        
        // Espera pelo menos um frame para garantir que todos os blocos foram destruídos
        yield return new WaitForEndOfFrame();
        
        // Espera adicional para garantir que não haja conflitos
        yield return null;
        
        // 2. Recriar todos os blocos originais
        Debug.Log($"Recriando {originalBricks.Count} blocos originais");
        
        foreach (BrickInfo brickInfo in originalBricks)
        {
            GameObject newBrick = Instantiate(
                brickTemplate, 
                brickInfo.position,
                brickInfo.rotation
            );
            
            newBrick.transform.localScale = brickInfo.scale;
            
            // Configurar o componente Brick com os dados originais
            Brick brickComponent = newBrick.GetComponent<Brick>();
            if (brickComponent != null)
            {
                brickComponent.breakSound = brickInfo.breakSound;
                
                // Garantir que o AudioSource seja configurado corretamente
                if (brickComponent.source == null)
                {
                    brickComponent.source = newBrick.GetComponent<AudioSource>();
                    if (brickComponent.source == null)
                    {
                        brickComponent.source = newBrick.AddComponent<AudioSource>();
                    }
                }
                
                brickComponent.source.playOnAwake = false;
                brickComponent.source.loop = false;
                brickComponent.source.volume = 1.0f;
                brickComponent.source.clip = brickInfo.breakSound;
            }
            
            Debug.Log($"Recriado bloco em: {brickInfo.position}");
        }
        
        // Verificação final
        yield return new WaitForEndOfFrame();
        
        int finalCount = 0;
        GameObject[] finalBricks = GameObject.FindGameObjectsWithTag("Brick");
        foreach (GameObject brick in finalBricks)
        {
            if (brick.transform.parent == null || !brick.transform.parent.CompareTag("Brick"))
            {
                finalCount++;
            }
        }
        
        Debug.Log($"Total de blocos raiz após recriação: {finalCount}");
    }
}