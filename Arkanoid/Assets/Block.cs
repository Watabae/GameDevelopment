using UnityEngine;

public class Brick : MonoBehaviour
{
    public AudioSource source;       // Fonte de áudio
    public AudioClip breakSound;     // Som ao quebrar o bloco

    void Start()
    {
        // Verifica se há um AudioSource no objeto, senão adiciona um
        if (source == null)
        {
            source = gameObject.AddComponent<AudioSource>();
        }
         // Configurações do AudioSource
        source.playOnAwake = false;  // Garante que o som não toque ao iniciar
        source.loop = false;         // O som não deve ficar em loop
        source.volume = 1.0f;        // Volume máximo

        if (breakSound != null)
        {
            source.clip = breakSound;  // Atribui o som ao AudioSource
        }
    }

    void OnCollisionEnter2D(Collision2D collision) 
    {
        if (collision.gameObject.CompareTag("Ball")) // Verifica se a bola colidiu com o bloco
        {
            if (source.clip != null)
            {
                source.Play(); // Toca o som da colisão
            }
        

            GameManager.AddScore(100); // Adiciona 100 pontos ao jogador

            Destroy(gameObject); // Destroi o bloco
        }
    }
}