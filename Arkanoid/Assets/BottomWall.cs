using UnityEngine;
using System.Collections;

public class SideWalls : MonoBehaviour 
{
    public AudioSource source;       // Fonte de áudio
    public AudioClip wallHitSound;   // Som ao colidir

    void Start()
    {
        if (source != null && wallHitSound != null)
        {
            source.clip = wallHitSound;  // Define o som da colisão
            source.playOnAwake = false;  // Garante que o som não toque automaticamente
        }
    }

    // Verifica colisões da bola nas paredes
    void OnTriggerEnter2D(Collider2D hitInfo) 
    {
        if (hitInfo.CompareTag("Ball"))
        {
            string wallName = transform.name;

            if (wallName == "BottomWall")
            {
                // Se for a parede inferior, ativa o Game Over
                FindFirstObjectByType<GameManager>().GameOver();
                hitInfo.gameObject.SetActive(false); // Desativa a bola ao perder
            }
            else
            {
                // Se for outra parede, adiciona pontuação e reseta a bola
                hitInfo.gameObject.SendMessage("RestartGame", null, SendMessageOptions.RequireReceiver);
            }

            // Toca o som da colisão
            if (source != null && source.clip != null)
            {
                source.Play();
            }
        }
    }
}