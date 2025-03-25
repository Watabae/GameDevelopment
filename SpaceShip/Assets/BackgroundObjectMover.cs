using UnityEngine;

public class BackgroundObjectMover : MonoBehaviour
{
    private float moveSpeed;
    private float parallaxEffect;
    private float speedMultiplier;

    // Método para configurar a velocidade do objeto
    public void SetSpeed(float speed)
    {
        moveSpeed = speed;
    }

    // Método para começar a movimentação
    public void StartMoving(float parallaxEffect, float speedMultiplier)
    {
        this.parallaxEffect = parallaxEffect;
        this.speedMultiplier = speedMultiplier;
    }

    void Update()
    {
        // Movimento do objeto no fundo com efeito parallax
        transform.position += Vector3.left * Time.deltaTime * moveSpeed * parallaxEffect * speedMultiplier;

        // Se o objeto sair da tela (à esquerda), destrua-o
        if (transform.position.x < -10f) // Destruir o objeto quando sair da tela (ajuste conforme necessário)
        {
            Destroy(gameObject);
        }
    }
}
