using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimentsJugadors : MonoBehaviour
{
    public float speed = 5f; // Velocidad de movimiento
    public float jumpForce = 7f; // Fuerza del salto
    public float climbSpeed = 4f; // Velocidad al subir escaleras

    private Rigidbody2D rb;
    private bool isGrounded = true;
    private bool onLadder = false;

    // Límites de la pantalla
    private float minX, maxX, minY, maxY;

    // Teclas para cada jugador
    private KeyCode leftKey;
    private KeyCode rightKey;
    private KeyCode upKey;
    private KeyCode downKey;

    private float halfWidth;
    private float halfHeight;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Asignar teclas según el tag
        if (CompareTag("Jugador1"))
        {
            leftKey = KeyCode.A;
            rightKey = KeyCode.D;
            upKey = KeyCode.W;
            downKey = KeyCode.S;
        }
        else if (CompareTag("Jugador2"))
        {
            leftKey = KeyCode.J;
            rightKey = KeyCode.L;
            upKey = KeyCode.I;
            downKey = KeyCode.K;
        }

        // Calcular límites de la pantalla usando la cámara principal
        Camera cam = Camera.main;
        Vector2 bottomLeft = cam.ViewportToWorldPoint(new Vector2(0, 0));
        Vector2 topRight = cam.ViewportToWorldPoint(new Vector2(1, 1));
        minX = bottomLeft.x;
        minY = bottomLeft.y;
        maxX = topRight.x;
        maxY = topRight.y;

        // Calcular la mitad del tamaño del personaje (Collider2D)
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            halfWidth = col.bounds.extents.x + 0.1f; // Añadir un pequeño margen
            halfHeight = col.bounds.extents.y + 0.1f; // Añadir un pequeño margen
        }
        else
        {
            halfWidth = 0.5f;
            halfHeight = 0.5f;
        }
    }

    void Update()
    {
        float moveX = 0f;
        float moveY = rb.velocity.y;

        // Movimiento horizontal
        if (Input.GetKey(leftKey))
            moveX = -1f;
        if (Input.GetKey(rightKey))
            moveX = 1f;

        // Movimiento en escalera
        if (onLadder)
        {
            rb.gravityScale = 0f;
            moveY = 0f;
            if (Input.GetKey(upKey))
                moveY = climbSpeed;
            else if (Input.GetKey(downKey))
                moveY = -climbSpeed;
            else
                moveY = 0f;

            rb.velocity = new Vector2(moveX * speed, moveY);
        }
        else
        {
            rb.gravityScale = 1f;
            // Salto (permite mantener presionada la tecla)
            if (Input.GetKey(upKey) && isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                isGrounded = false;
            }
            else
            {
                rb.velocity = new Vector2(moveX * speed, rb.velocity.y);
            }
        }

        // Limitar la posición del jugador a los límites de la pantalla con margen del tamaño del personaje
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX + halfWidth, maxX - halfWidth);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, minY + halfHeight, maxY - halfHeight);

        // Si toca el límite superior, anula la velocidad vertical positiva para evitar que "flote"
        if (clampedPosition.y >= maxY - halfHeight && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
        }

        transform.position = clampedPosition;
    }

    // Detectar si está en el suelo con cualquier objeto excepto escaleras
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Escales") && collision.contacts[0].normal.y > 0.5f)
        {
            isGrounded = true;
        }
    }

    // Detectar si está en la escalera
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Escales"))
        {
            onLadder = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Escales"))
        {
            onLadder = false;
            rb.gravityScale = 1f;
        }
    }
}
