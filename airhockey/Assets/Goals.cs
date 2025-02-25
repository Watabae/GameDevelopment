using UnityEngine;
using System.Collections;

public class SideWalls : MonoBehaviour 
{
    public AudioSource source;       // Fonte de áudio
    public AudioClip wallHitSound;   // Som ao colidir

    void Start()
    {
        source.clip = wallHitSound;  // Define o som da colisão
        source.playOnAwake = false;  // Garante que o som não toque automaticamente
    }

    // Verifica colisões da bola nas paredes
    void OnTriggerEnter2D(Collider2D hitInfo) 
    {
        if (hitInfo.tag == "Ball")
        {
            string wallName = transform.name;
            GameManager.Score(wallName);
            hitInfo.gameObject.SendMessage("RestartGame", null, SendMessageOptions.RequireReceiver);

            // Toca o som da colisão com a parede
            if (source.clip != null)
            {
                source.Play();
            }
        }
    }
}
