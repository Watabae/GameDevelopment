using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    public float speed = 10.0f;  // Define a velocidade da raquete
    public float boundX = 8.0f;  // Define os limites horizontais
    public float minY = -4.0f;   // Limite inferior para a raquete de baixo
    public float maxY = 4.0f;    // Limite superior para a raquete de cima
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
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var pos = transform.position;
        
        // Atualiza posição baseada no mouse
        pos.x = mousePos.x;
        pos.y = mousePos.y;

        // Limita a posição dentro dos bounds definidos
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
