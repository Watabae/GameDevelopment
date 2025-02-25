using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static int PlayerScore1 = 0; // Pontuação do player 1
    public static int PlayerScore2 = 0; // Pontuação do player 2

    public GUISkin layout;              // Fonte do placar
    public int scoreFontSize = 48;     // Tamanho da fonte para a pontuação
    GameObject theBall;                 // Referência ao objeto bola

    // Start is called before the first frame update
    void Start()
    {
        theBall = GameObject.FindGameObjectWithTag("Ball"); // Busca a referência da bola
    }
    // incrementa a potuação
    public static void Score (string wallID) {
        if (wallID == "BottomGoal")
        {
            PlayerScore1++;
        } else
        {
            PlayerScore2++;
        }
    }

    // Gerência da pontuação e fluxo do jogo
    void OnGUI () {
        GUI.skin = layout;

        // Cria um estilo para a pontuação
        GUIStyle scoreStyle = new GUIStyle(GUI.skin.label);
        scoreStyle.fontSize = scoreFontSize; // Define o tamanho da fonte

        GUI.Label(new Rect(Screen.width / 2 - 370 - 12, 100, 100, 100), "" + PlayerScore1, scoreStyle);
        GUI.Label(new Rect(Screen.width / 2 - 370 - 12, 380, 100, 100), "" + PlayerScore2, scoreStyle);

        if (GUI.Button(new Rect(Screen.width / 2 - 400 - 12, 275, 120, 53), "RESTART"))
        {
            PlayerScore1 = 0;
            PlayerScore2 = 0;
            theBall.SendMessage("RestartGame", null, SendMessageOptions.RequireReceiver);
        }
        if (PlayerScore1 == 10)
        {
            GUI.Label(new Rect(Screen.width / 2 - 150, 200, 2000, 1000), "PLAYER TWO WINS");
            theBall.SendMessage("ResetBall", null, SendMessageOptions.RequireReceiver);
        } else if (PlayerScore2 == 10)
        {
            GUI.Label(new Rect(Screen.width / 2 - 150, 200, 2000, 1000), "PLAYER ONE WINS");
            theBall.SendMessage("ResetBall", null, SendMessageOptions.RequireReceiver);
        }
    }
    
    // Método para exibir texto verticalmente
    private void DisplayVerticalText(string text, Vector2 position, GUIStyle style)
    {
        // Salva a matriz de transformação atual
        Matrix4x4 oldMatrix = GUI.matrix;
        
        // Move a matriz de transformação para a posição desejada
        GUIUtility.RotateAroundPivot(90, position); // Rotaciona 90 graus em torno do ponto especificado
        GUI.Label(new Rect(position.x, position.y, 200, 200), text, style); // Desenha o texto

        // Restaura a matriz de transformação
        GUI.matrix = oldMatrix;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
