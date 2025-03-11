using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    public KeyCode moveRight = KeyCode.D;      // Move a raquete para cima
    public KeyCode moveLeft = KeyCode.A;    // Move a raquete para baixo
    public float speed = 10.0f;  // Define a velocidade da raquete
    public float minX = -12.6f;  // Define o limite mínimo em x
    public float maxX = 4.6f;    // Define o limite máximo em x
    private Rigidbody2D rb2d;

    public AudioSource source;  // Fonte de áudio
    public AudioClip hitSound;  // Som do impacto

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>(); // Inicializa a raquete
        source.clip = hitSound;  // Define o som
        source.playOnAwake = false; // Garante que o som não toque automaticamente
    }

    void Update()
    {
        var vel = rb2d.velocity;                // Acessa a velocidade da raquete

        if (Input.GetKey(moveRight)) {             // Velocidade da Raquete para ir para cima
            vel.x = speed;
        }
        else if (Input.GetKey(moveLeft)) {      // Velocidade da Raquete para ir para cima
            vel.x = -speed;                    
        }
        else {
            vel.x = 0;                          // Velociade para manter a raquete parada
        }

        rb2d.velocity = vel;                    // Atualizada a velocidade da raquete

        var pos = transform.position;           // Acessa a Posição da raquete

        // Limita a posição dentro dos bounds definidos
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
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