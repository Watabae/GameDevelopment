using UnityEngine;
using UnityEngine.SceneManagement;

public class TrocaDeFase : MonoBehaviour
{
    public string Fase2; // Nome da próxima cena

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Entrou no trigger!"); // <==== DEIXE ESTA LINHA PARA TESTE
            SceneManager.LoadScene(Fase2);
        }
    }
}