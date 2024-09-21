using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    private bool isGrounded;

    private void Start()
    {
        myCharacterController = GetComponent<CharacterController>();   
        
        // Ocultar el cursor al iniciar el juego
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
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
}
