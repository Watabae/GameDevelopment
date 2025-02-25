using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    public float speed = 10.0f;             // Define a velocidade da bola
    public float boundY = 4;            // Define os limites em Y
    private Rigidbody2D rb2d;    

    // Start is called before the first frame update
    void Start()
    {
      rb2d = GetComponent<Rigidbody2D>(); // inicia a raquete  
    }

    // Update is called once per frame
    void Update () {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var pos = transform.position;
        pos.x = mousePos.x;
        pos.y = mousePos.y;
        transform.position = pos;   
                
        if (pos.y > boundY) {                  
            pos.y = boundY;                     // Corrige a posicao da raquete caso ele ultrapasse o limite superior
        }
        else if (pos.y < -boundY) {
            pos.y = -boundY;                    // Corrige a posicao da raquete caso ele ultrapasse o limite inferior
        }
        transform.position = pos;               // Atualiza a posição da raquete
    }

}
