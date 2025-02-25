using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIControls : MonoBehaviour
{
    public Transform ball;       // Referência para a bola
    public float speed = 5.0f;   // Velocidade da IA
    public float boundX = 8.0f;  // Limite horizontal
    public float minY = -4.0f;   // Limite inferior (para raquete de baixo)
    public float maxY = 4.0f;    // Limite superior (para raquete de cima)

    public AudioSource source;   // Fonte de áudio
    public AudioClip hitSound;   // Som ao colidir

    void Start()
    {
        source.clip = hitSound;  // Define o som da colisão
        source.playOnAwake = false; // Garante que o som não toque automaticamente
    }

    void Update()
    {
        if (ball == null) return; // Garante que a bola foi atribuída
        
        Vector3 pos = transform.position;
        
        // A IA tenta seguir a posição X da bola
        pos.x = Mathf.Lerp(pos.x, ball.position.x, speed * Time.deltaTime);
        pos.y = Mathf.Lerp(pos.y, ball.position.y, speed * Time.deltaTime/2);

        // Mantém a raquete dentro dos limites estabelecidos
        pos.x = Mathf.Clamp(pos.x, -boundX, boundX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        // Aplica a posição corrigida
        transform.position = pos;
    }

    void OnCollisionEnter2D(Collision2D coll) 
    {
        if (source.clip != null)
        {
            source.Play(); // Toca o som ao colidir
        }
    }
}
