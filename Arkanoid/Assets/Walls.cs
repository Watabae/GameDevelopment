using UnityEngine;

public class WallSound : MonoBehaviour 
{
    public AudioSource source;       // Componente de áudio
    public AudioClip wallHitSound;   // Som ao tocar na parede
    
    // NOVA VARIÁVEL: Indica se esta é a parede inferior (que tira vidas)
    public bool isBottomWall = false; // Marque esta opção no Inspector para a parede inferior
    
    // NOVO ÁUDIO: Som específico para quando a bola cai (opcional)
    public AudioClip fallSound;      // Som para quando a bola cai (parede inferior)
    
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
        if (wallHitSound != null)
        {
            source.clip = wallHitSound;  // Atribui o som ao AudioSource
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision) 
    {
        if (collision.gameObject.CompareTag("Ball")) // Verifica se a bola colidiu com a parede
        {
            // Se for a parede inferior, reduz uma vida
            if (isBottomWall)
            {
                // Encontra o GameManager para reduzir a vida
                GameManager gameManager = FindObjectOfType<GameManager>();
                if (gameManager != null)
                {
                    // Tocar o som de queda, se disponível, caso contrário usa o som padrão da parede
                    if (fallSound != null)
                    {
                        source.clip = fallSound;
                    }
                    
                    if (source.clip != null)
                    {
                        source.Play(); // Toca o som da colisão/queda
                    }
                    
                    // Chama o método para perder uma vida
                    gameManager.LoseLife();
                }
            }
            else
            {
                // Comportamento padrão para outras paredes
                if (source.clip != null)
                {
                    source.Play(); // Toca o som da colisão
                }
            }
        }
    }
}