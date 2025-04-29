using UnityEngine;
using UnityEngine.UI;

public class SignInteraction : MonoBehaviour
{
    public GameObject caixaTexto;         // UI do painel de fala
    public string mensagem;               // Mensagem a ser exibida
    private bool jogadorNaArea;
    private bool lendoMensagem;

    void Update()
    {
        if (jogadorNaArea && Input.GetKeyDown(KeyCode.Z))
        {
            if (!lendoMensagem)
            {
                caixaTexto.SetActive(true);
                caixaTexto.GetComponentInChildren<Text>().text = mensagem;
                lendoMensagem = true;
            }
        }
        else if (lendoMensagem && Input.anyKeyDown)
        {
            caixaTexto.SetActive(false);
            lendoMensagem = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            jogadorNaArea = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            jogadorNaArea = false;
    }
}
