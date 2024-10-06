/*
hace un componente para que mi personaje de unity (una capsula) se mueva correctamente y salte.  
En su visor (elemento del personaje) 
debe tener una camara que funcionara para que el usuario pueda ver a traves de ella.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CharacterController))]
public class ControladorMovimientoPersonaje : MonoBehaviour
{
    public float moveSpeed = 5f; // Velocidad de movimiento
    public float jumpForce = 7f; // Fuerza de salto
    public Transform cameraTransform; // Referencia a la cámara del personaje
    public float mouseSensitivity = 2f; // Sensibilidad del mouse para rotar la cámara

    private Rigidbody rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked; // Bloquea el cursor en el centro de la pantalla
        Cursor.visible = false; // Oculta el cursor
    }

    void Update()
    {
        Move();
        RotateCamera();
        Jump();
    }

    void Move()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        rb.MovePosition(transform.position + move * moveSpeed * Time.deltaTime);
    }

    void RotateCamera()
    {
        // Rotación de la cámara en el eje X (mirar hacia arriba y abajo)
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        cameraTransform.Rotate(Vector3.left * mouseY);

        // Rotación del personaje en el eje Y (girar izquierda/derecha)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(Vector3.up * mouseX);
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Detecta si el personaje está tocando el suelo
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
