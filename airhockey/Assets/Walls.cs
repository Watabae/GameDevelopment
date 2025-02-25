using UnityEngine;

public class WallSound : MonoBehaviour 
{
    public AudioSource source;       // Componente de áudio
    public AudioClip wallHitSound;   // Som ao tocar na parede

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

        if (wallHitSound != null)
        {
            source.clip = wallHitSound;  // Atribui o som ao AudioSource
        }
    }

    void OnCollisionEnter2D(Collision2D collision) 
    {
        if (collision.gameObject.CompareTag("Ball")) // Verifica se a bola colidiu com a parede
        {
            if (source.clip != null)
            {
                source.Play(); // Toca o som da colisão
            }
        }
    }
}
