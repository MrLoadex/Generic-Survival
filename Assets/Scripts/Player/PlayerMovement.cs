using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(CharacterController))]

public class PlayerMovement : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float rotationSensitivity = 100f;
    [SerializeField] private float jumpForce = 3f;

    [Header("Configuración de Cámara")]
    [SerializeField] private Transform mainCamera;

    [Header("Configuración de Física")]
    [SerializeField] private float gravityMultiplier = 2.5f;

    private CharacterController myCharacterController;
    private Vector3 velocity;
    private float originalWalkSpeed;
    private float originalRunSpeed;
    private bool isGrounded = true;
    private float lastHeightGrounded;
    private bool isFalling = false;
    private bool canMove = true;
    private bool canRotate = true;
    private PlayerController playerController = new PlayerFootController();

    // Limitantes de velocidad
    bool isExhausted = false;
    bool isOverweight = false;

    private void Start()
    {
        myCharacterController = GetComponent<CharacterController>();   

        originalWalkSpeed = walkSpeed;
        originalRunSpeed = runSpeed;
    }

    private void Update()
    {
        CheckGrounded();
        CheckFall();
        if (canMove) {
            ProcessMovement();
        }
        if (canRotate) {
            ProcessRotation();
        }
    }

    private void CheckGrounded()
    {
        isGrounded = myCharacterController.isGrounded;
        if (isGrounded && velocity.y < 0) 
        { 
            velocity.y = -2f; // CORREGIR BUG DE CAIDA AL VACIO
        }
    }
    // Metodo para procesar el movimiento
    private void ProcessMovement()
    {
        // Obtener entrada horizontal y vertical
        float horizontalMovement = playerController.getHorizontalAxis();
        float verticalMovement = playerController.getVerticalAxis();

        // Calcular el vector de movimiento
        Vector3 movement = transform.right * horizontalMovement + transform.forward * verticalMovement;

        // Determinar la velocidad actual (caminar o correr)
        float currentSpeed = playerController.getRunInput() ? runSpeed : walkSpeed;

        // Llamar al evento de consumo de stats si está corriendo
        if (playerController.getRunInput())
        {
            PlayerEvents.Run?.Invoke(); // Ajusta el valor según sea necesario
        }

        // Aplicar el movimiento al CharacterController
        myCharacterController.Move(movement * currentSpeed * Time.deltaTime);
        // Salto
        if (playerController.getJumpInput() && isGrounded)
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
        float mouseX = playerController.getRotationXAxis() * rotationSensitivity * Time.deltaTime;
        float mouseY = playerController.getRotationYAxis() * rotationSensitivity * Time.deltaTime;

        // Rotar el jugador horizontalmente
        transform.Rotate(Vector3.up * mouseX);

        // Limitar la rotación vertical para evitar que la cámara gire completamente
        mouseY = Mathf.Clamp(mouseY, -90f, 90f);

        // Aplicar la rotación vertical a la cámara
        mainCamera.localRotation = Quaternion.Euler(mainCamera.localRotation.eulerAngles.x - mouseY, 0f, 0f);
    }

    private void CheckFall()
    {
        if (!isGrounded && !isFalling && velocity.y < 0)
        {
            isFalling = true;
            lastHeightGrounded = transform.position.y;
        }

        if (isFalling && isGrounded)
        {
            isFalling = false;
            float fallHeight = Mathf.Abs(transform.position.y - lastHeightGrounded);
            PlayerEvents.Fall?.Invoke(fallHeight);
        }
    }

    private void CalculateMovementSpeed()
    {
        int debuff = (isOverweight ? 1 : 0) + (isExhausted ? 1 : 0);
        walkSpeed = originalWalkSpeed / (1 + debuff);
        runSpeed = originalRunSpeed / (1 + debuff);
    }

    #region EVENTS

    private void OnEnable() {
        // Escuchar el evento de muerte del jugador
        PlayerEvents.Death += replyPlayerDeath;
        PlayerEvents.Revive += replyPlayerRevive;
        PlayerEvents.Exhausted += replyPlayerExhausted;
        PlayerEvents.Rested += replyPlayerEnergized;
        InventoryEvents.OpenCloseInventory += replyOpenCloseInventory;
        PlayerEvents.Fall += replyPlayerFall;
    }

    private void OnDisable() {
        PlayerEvents.Death -= replyPlayerDeath;
        PlayerEvents.Revive -= replyPlayerRevive;
        PlayerEvents.Exhausted -= replyPlayerExhausted;
        PlayerEvents.Rested -= replyPlayerEnergized;
        InventoryEvents.OpenCloseInventory -= replyOpenCloseInventory;
        PlayerEvents.Fall -= replyPlayerFall;
    }  

    private void OnDestroy() {
        PlayerEvents.Death -= replyPlayerDeath;
        PlayerEvents.Revive -= replyPlayerRevive;
        PlayerEvents.Exhausted -= replyPlayerExhausted;
        PlayerEvents.Rested -= replyPlayerEnergized;
        InventoryEvents.OpenCloseInventory -= replyOpenCloseInventory;
        PlayerEvents.Fall -= replyPlayerFall;
    }

    // Metodo para manejar el evento de muerte del jugador
    private void replyPlayerDeath()
    {
        canMove = false;
        canRotate = false;
    }

    private void replyPlayerRevive()
    {
        canMove = true;
        canRotate = true;
    }
    
    private void replyPlayerExhausted()
    {
        isExhausted = true;
        CalculateMovementSpeed();
    }
    
    private void replyPlayerEnergized()
    {
        isExhausted = false;
        CalculateMovementSpeed();
    }

    private void replyOpenCloseInventory(bool isOpen)
    {
        if (isOpen) 
        {
            canRotate = false;  
        } else 
        {
            canRotate = true;
        }
    }
    
    private void replyPlayerFall(float fallHeight)
    {
        if (fallHeight > 4f)
        {
           // movimiento super lento por medio segundo
           StartCoroutine(SlowMovement(fallHeight));
        }
    }

    IEnumerator SlowMovement(float fallHeight)
    {
        walkSpeed /= fallHeight / 2;
        runSpeed /= fallHeight / 2;
        yield return new WaitForSeconds(fallHeight / 10); // 4 = 0.4s y 10 = 1s
        CalculateMovementSpeed();
    }
    #endregion
}
