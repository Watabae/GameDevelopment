using UnityEngine;
using UnityEngine.SceneManagement; // Importar o namespace para gerenciar cenas

public class MenuManager : MonoBehaviour
{
    // Método para carregar a cena do jogo
    public void StartGame()
    {
        SceneManager.LoadScene("Game"); // 
    }
}
