using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject tutorialPanel;
    public string nomeCenaDoJogo = "Fase1";

    void Update()
    {
        if (tutorialPanel.activeSelf && Input.GetKeyDown(KeyCode.Q))
        {
            FecharTutorial();
        }
    }

    public void IniciarJogo()
    {
        SceneManager.LoadScene(nomeCenaDoJogo);
    }

    public void AbrirTutorial()
    {
        tutorialPanel.SetActive(true);
    }

    public void FecharTutorial()
    {
        tutorialPanel.SetActive(false);
    }
}
