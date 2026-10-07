using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float moveSpeed;

    [SerializeField] private float jumpHeight; // Altura do pulo (Medida em metros) ex: se for 2, pula 2 metros

    private Vector3 velocity;
    private Vector3 moveDirection;

    private float horizontal;
    private float vertical;

    private void Update()
    {
        GetInput(); // Pegar o input
        Gravity(); // Calcular a gravidade
        Move(); // Movimentar o jogador
        Jump();
    }

    private void GetInput()
    {
        // Pegamos o valor dos botões WASD para controlar nosso personagem depois
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
    }

    private void Gravity()
    {
        // Verifica se o jogador estava caindo e está no chão
        if(controller.isGrounded && velocity.y < 0) 
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
    }

    private void Move()
    {
        moveDirection = transform.right * horizontal + transform.forward * vertical;
        moveDirection.Normalize();

        Vector3 movement = moveDirection * moveSpeed;
        movement.y = velocity.y;

        controller.Move(movement * Time.deltaTime);
    }

    private void Jump()
    {
        if(Input.GetKeyDown(KeyCode.Space) && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

}
