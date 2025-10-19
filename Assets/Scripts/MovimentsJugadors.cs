using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimentsJugadors : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Moviment Horizontal Jugador")]
    private float movimentHorizontal = 0f;
    [SerializeField] private float velocitatMoviment;
    [Range(0f, 0.3f)] [SerializeField] private float suavitzat;

    private Vector3 velocitat = Vector3.zero;
    private bool miraDreta = true;

    [Header("Salt Jugador")]
    [SerializeField] private float forcaSalt;
    [SerializeField] private LayerMask queEsTerra;
    [SerializeField] private Transform controladorTerra;
    [SerializeField] private Vector3 midaCaixaControlador;
    [SerializeField] private bool estaAterra;
    private bool salt = false;

    [Header("Ajupit")]
    private bool ajupit = false;

    [Header("Escales")]
    private float movimentVertical = 0f;
    private bool escales = false;
    [SerializeField] private float velocitatEscales;
    [SerializeField] private Transform centroEscalera;

    [Header("Tecles")]
    private KeyCode leftKey;
    private KeyCode rightKey;
    private KeyCode upKey;
    private KeyCode downKey;
    private KeyCode specialKey;

    [Header("Limits pantalla")]
    private float minX, maxX, minY, maxY;

    [Header("Animacions")]
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        assignarTecles();
        calcularLimitsPantalla();
    }

    void Update()
    {
        //Moviment horitzontal
        if (escales && rb.gravityScale == 0f)
        {
            // Si está en la escalera y enganchado, no puede moverse derecha/izquierda
            movimentHorizontal = 0f;
            // Si pulsa izquierda o derecha, se desengancha de la escalera
            if (Input.GetKeyDown(leftKey) || Input.GetKeyDown(rightKey))
            {
                rb.gravityScale = 1f;
            }
        }
        else {
            if (Input.GetKey(leftKey))
                movimentHorizontal = -velocitatMoviment;
            else if (Input.GetKey(rightKey))
                movimentHorizontal = velocitatMoviment;
            else
                movimentHorizontal = 0f; 
        }
        
        animator.SetFloat("Horizontal", Mathf.Abs(movimentHorizontal));

        animator.SetFloat("VelocitatY", rb.velocity.y);

        // Engancharse a la escalera si está en contacto y pulsa up o down
        if (escales && (Input.GetKeyDown(upKey) || Input.GetKeyDown(downKey)))
        {
            rb.gravityScale = 0f;
            rb.velocity = Vector2.zero;   

            // Centrar al jugador en la escalera
            if (centroEscalera != null) {
                Vector3 pos = transform.position;
                pos.x = centroEscalera.position.x;
                transform.position = pos;
            }
        }

        // Movimiento vertical en escalera solo si está enganchado (sin gravedad)
        if (escales && rb.gravityScale == 0f)
        {
            if (Input.GetKey(upKey)) {
                movimentVertical = velocitatEscales;
            }
            else if (Input.GetKey(downKey))
                movimentVertical = -velocitatEscales;
            else
                movimentVertical = 0f;
        }
        

        // Salt
        if (Input.GetKeyDown(upKey))
        {
            salt = true;
        } 

        // Ajupit
        ajupit = Input.GetKey(downKey);
    }

    private void FixedUpdate()
    {
        estaAterra = Physics2D.OverlapBox(controladorTerra.position, midaCaixaControlador, 0f, queEsTerra);
        animator.SetBool("estaAterra", estaAterra);
        animator.SetBool("estaAjupit", ajupit);
        animator.SetBool("estaEscales", escales && rb.gravityScale == 0f);
        //Moure el jugador
        if (escales && rb.gravityScale == 0f)
        {
            // Movimiento vertical y horizontal en escalera
            rb.velocity = new Vector2(movimentHorizontal * Time.fixedDeltaTime, movimentVertical);
        }
        else
        {
            // Movimiento normal fuera de escalera
            MoureJugador(movimentHorizontal * Time.fixedDeltaTime, salt);
        }
            salt = false;
        
    }

    private void assignarTecles() {
        // Asignar teclas según el tag
        if (CompareTag("Jugador1"))
        {
            leftKey = KeyCode.A;
            rightKey = KeyCode.D;
            upKey = KeyCode.W;
            downKey = KeyCode.S;
            specialKey = KeyCode.E;
        }
        else if (CompareTag("Jugador2"))
        {
            leftKey = KeyCode.J;
            rightKey = KeyCode.L;
            upKey = KeyCode.I;
            downKey = KeyCode.K;
            specialKey = KeyCode.O;
        }
    }

    private void calcularLimitsPantalla()
    {
        Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        minX = bottomLeft.x;
        minY = bottomLeft.y;
        maxX = topRight.x;
        maxY = topRight.y;
    }

    private void MoureJugador(float moure, bool saltar)
    {
        Vector3 velocitatObjectiu = new Vector2(moure, rb.velocity.y);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, velocitatObjectiu, ref velocitat, suavitzat);
        
        if (moure > 0 && !miraDreta)
        {
            Girar();
        }
        else if (moure < 0 && miraDreta)
        {
            Girar();
        }

        if (estaAterra && saltar && rb.gravityScale == 1f)
        {
            estaAterra = false;
            rb.AddForce(new Vector2(0f, forcaSalt));
        }
    }
    private void Girar()
    {
        miraDreta = !miraDreta;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Escales"))
        {
            escales = true;
            centroEscalera = other.transform.Find("CentreEscales");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Escales"))
        {
            escales = false;
            rb.gravityScale = 1f;
            centroEscalera = null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(controladorTerra.position, midaCaixaControlador);
    }

}
