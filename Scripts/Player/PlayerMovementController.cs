/*
Crea un controlador de movimiento para un jugador en primera persona en Unity con las siguientes características:
- Movimiento básico (caminar y correr)
- Salto
- Rotación de cámara con el ratón
- Gravedad personalizada
- Comprobación de si está en el suelo
- Ocultar y bloquear el cursor del ratón
- Utiliza CharacterController para el movimiento
- Incluye SerializeField para ajustar parámetros desde el Inspector
- Escucha el evento de muerte del jugador y se desactiva cuando se invoca
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(CharacterController))]

public class PlayerMovementController : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private float jumpForce = 3f;

    [Header("Configuración de Cámara")]
    [SerializeField] private Transform mainCamera;

    [Header("Configuración de Física")]
    [SerializeField] private float gravityMultiplier = 2.5f;

    private CharacterController myCharacterController;
    private Vector3 velocity;
    private bool isGrounded = true;
    private float originalWalkSpeed;
    private float originalRunSpeed;

    private void Start()
    {
        myCharacterController = GetComponent<CharacterController>();   
        //
        originalWalkSpeed = walkSpeed;
        originalRunSpeed = runSpeed;
        // Ocultar el cursor al iniciar el juego
        /*ESTO NO DEBERIA ESTAR ACA, PERO POR AHORA ACA SE QUEDA*/
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Escuchar el evento de muerte del jugador
        PlayerEvents.Death += replyPlayerDeath;
        PlayerEvents.Revive += replyPlayerRevive;
        PlayerEvents.Exhausted += replyPlayerExhausted;
        PlayerEvents.Rested += replyPlayerEnergized;
    }

    private void Update()
    {
        CheckGrounded();
        ProcessMovement();
        ProcessRotation();
    }

    private void CheckGrounded()
    {
        isGrounded = myCharacterController.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }
    // Metodo para procesar el movimiento
    private void ProcessMovement()
    {
        // Obtener entrada horizontal y vertical
        float horizontalMovement = Input.GetAxis("Horizontal");
        float verticalMovement = Input.GetAxis("Vertical");

        // Calcular el vector de movimiento
        Vector3 movement = transform.right * horizontalMovement + transform.forward * verticalMovement;

        // Determinar la velocidad actual (caminar o correr)
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        // Llamar al evento de consumo de stats si está corriendo
        if (Input.GetKey(KeyCode.LeftShift))
        {
            PlayerEvents.Run?.Invoke(); // Ajusta el valor según sea necesario
        }

        // Aplicar el movimiento al CharacterController
        myCharacterController.Move(movement * currentSpeed * Time.deltaTime);
        // Salto
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y);
        }

        // Aplicar gravedad
        velocity.y += Physics.gravity.y * gravityMultiplier * Time.deltaTime;

        // Mover al jugador
        myCharacterController.Move(velocity * Time.deltaTime);
    }

    private void ProcessRotation()
    {
        // Obtener la entrada del mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotar el jugador horizontalmente
        transform.Rotate(Vector3.up * mouseX);

        // Rotar la cámara verticalmente
        float xRotation = mainCamera.localEulerAngles.x - mouseY;
        
        // Limitar la rotación vertical para evitar que la cámara gire completamente
        if (xRotation > 180f) xRotation -= 360f;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        mainCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    #region EVENTS
    // Metodo para manejar el evento de muerte del jugador
    private void replyPlayerDeath()
    {
        this.enabled = false;
        Cursor.lockState = CursorLockMode.None;
    }

    private void replyPlayerRevive()
    {
        Cursor.lockState = CursorLockMode.Locked;
        this.enabled = true;
    }
    
    private void replyPlayerExhausted()
    {
        //disminuir la velocidad de movimiento
        walkSpeed = originalWalkSpeed / 2;
        runSpeed = originalRunSpeed / 2;
        //Añadir animacion de cansancio
    }
    
    private void replyPlayerEnergized()
    {
        //aumentar la velocidad de movimiento
        walkSpeed = originalWalkSpeed;
        runSpeed = originalRunSpeed;
    }
    #endregion
}
