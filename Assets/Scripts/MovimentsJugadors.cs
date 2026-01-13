using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controlador de moviments dels jugadors.
/// Gestiona el moviment horitzontal, salt, interacció amb escales i animacions.
/// Suporta dos jugadors amb tecles diferents segons el tag (Jugador1 o Jugador2).
/// </summary>
public class MovimentsJugadors : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField] private bool socJugador1;

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
    private float gravetat = 1.5f;
    private bool salt = false;

    [Header("Escales")]
    private float movimentVertical = 0f;
    private bool escales = false;
    [SerializeField] private float velocitatEscales;
    [SerializeField] private Transform centreEscales;

    [Header("Tecles")]
    private KeyCode leftKey;
    private KeyCode rightKey;
    private KeyCode upKey;
    private KeyCode downKey;

    [Header("Agafar Objectes")]
    private AgafarObjecte agafarObjecte;

    [Header("Animacions")]
    private Animator animator;

    /// <summary>
    /// Inicialitza els components i assigna les tecles segons el jugador.
    /// Determina si és Jugador1 o Jugador2 segons el tag de l'objecte.
    /// </summary>
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.gravityScale = gravetat;
        socJugador1 = CompareTag("Jugador1");
        assignarTecles();
        agafarObjecte = GetComponentInChildren<AgafarObjecte>();
    }

    /// <summary>
    /// Processa l'input del jugador cada frame.
    /// Gestiona el moviment horitzontal, vertical (escales), salt i animacions.
    /// </summary>
    void Update()
    {
        // Moviment horitzontal
        processarInputHoritzontal();

        // Moviment escales
        processarInputEscales();
        
        // Salt
        if (Input.GetKeyDown(upKey))
            salt = true;

        // Animacions
        animator.SetFloat("Horizontal", Mathf.Abs(movimentHorizontal));
        animator.SetFloat("VelocitatY", rb.velocity.y);
    }

    /// <summary>
    /// Aplica el moviment físic del jugador cada frame fixat.
    /// Comprova si el jugador està a terra, actualitza animacions i aplica forces.
    /// </summary>
    private void FixedUpdate()
    {
        // Comprovar si està a terra
        comprovarEstaAterra();

        // Actualitzar estats animacions
        animator.SetBool("estaAterra", estaAterra);
        animator.SetBool("estaEscales", escales && rb.gravityScale == 0f);

        // Aplicar moviment
        if (escales && rb.gravityScale == 0f)
        {
            // Moviment vertical a escales
            rb.velocity = new Vector2(movimentHorizontal * Time.fixedDeltaTime, movimentVertical);
        }
        else
        {
            // Moviment horitzontal i salt
            executarMoviment(movimentHorizontal * Time.fixedDeltaTime, salt);
        }
        
        salt = false;
        
    }

    /// <summary>
    /// Assigna les tecles de control segons el jugador.
    /// Jugador1: A/D (horitzontal), W/S (vertical)
    /// Jugador2: Fletxes esquerra/dreta (horitzontal), Fletxes amunt/avall (vertical)
    /// </summary>
    private void assignarTecles() 
    {
        // Asignar tecles segons el tag
        if (socJugador1)
        {
            leftKey = KeyCode.A;
            rightKey = KeyCode.D;
            upKey = KeyCode.W;
            downKey = KeyCode.S;
        }
        else 
        {
            leftKey = KeyCode.LeftArrow;
            rightKey = KeyCode.RightArrow;
            upKey = KeyCode.UpArrow;
            downKey = KeyCode.DownArrow;
        }
    }

    /// <summary>
    /// Processa l'input de moviment horitzontal del jugador.
    /// Si està enganxat a una escala, no permet moviment horitzontal llevat que premi les tecles per desenganxar-se.
    /// </summary>
    private void processarInputHoritzontal() {
        // Si està a l'escala i enganxat, no pot moure's dreta/esquerra
        if (escales && rb.gravityScale == 0f)
        {
            movimentHorizontal = 0f;

            // Si prem esquerra o dreta, es desenganxa de l'escala
            if (Input.GetKeyDown(leftKey) || Input.GetKeyDown(rightKey))
            {
                rb.gravityScale = gravetat;
            }
        }
        // Moviment horitzontal fora de l'escala
        else {
            if (Input.GetKey(leftKey))
                movimentHorizontal = -velocitatMoviment;
            else if (Input.GetKey(rightKey))
                movimentHorizontal = velocitatMoviment;
            else
                movimentHorizontal = 0f; 
        }
    }

    /// <summary>
    /// Processa l'input de moviment vertical per a les escales.
    /// Els jugadors no poden pujar escales si estan agafant un objecte.
    /// Quan es prem amunt o avall estant en contacte amb una escala, el jugador s'hi enganxa.
    /// </summary>
    private void processarInputEscales()
    {
        // Si està agafant un objecte, no es pot enganxar a l'escala
        if (agafarObjecte != null && agafarObjecte.teObjecte)
        {
            movimentVertical = 0f;
            return; 
        }

            // Enganxar-se a l'escala si està en contacte i prem up o down
        if (escales && (Input.GetKeyDown(upKey) || Input.GetKeyDown(downKey)))
        {
            rb.gravityScale = 0f;
            rb.velocity = Vector2.zero;   

            // Centrar al jugador en la escalera
            if (centreEscales != null) {
                Vector3 pos = transform.position;
                pos.x = centreEscales.position.x;
                transform.position = pos;
            }
        }

        // Moviment vertical a l'escala només si està enganxat
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
    }

    /// <summary>
    /// Comprova si el jugador està en contacte amb terra, objectes o l'altre jugador.
    /// Utilitza Physics2D.OverlapBoxAll per detectar col·lisions a la base del jugador.
    /// Els jugadors poden estar a sobre l'un de l'altre.
    /// </summary>
    private void comprovarEstaAterra()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(controladorTerra.position, midaCaixaControlador, 0f);
        estaAterra = false;

        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject) continue; // Ignorarme a mí mismo

            // Detecta suelo: objetos, tilemap o jugadores distintos
            if (hit.CompareTag("Objecte") || hit.CompareTag("Terra") || 
            (socJugador1 && hit.CompareTag("Jugador2")) || 
            (!socJugador1 && hit.CompareTag("Jugador1")))
            {
                estaAterra = true;
                break;
            }
        }
    }

    /// <summary>
    /// Executa el moviment horitzontal i el salt del jugador.
    /// Aplica suavitzat al moviment i gira el sprite segons la direcció.
    /// </summary>
    /// <param name="moure">Quantitat de moviment horitzontal a aplicar.</param>
    /// <param name="saltar">Si el jugador ha de saltar en aquest frame.</param>
    private void executarMoviment(float moure, bool saltar)
    {
        // Suavitzar el moviment
        Vector3 velocitatObjectiu = new Vector2(moure, rb.velocity.y);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, velocitatObjectiu, ref velocitat, suavitzat);
        
        // Girar el jugador segons la direcció
        if (moure > 0 && !miraDreta)
        {
            Girar();
        }
        else if (moure < 0 && miraDreta)
        {
            Girar();
        }

        // Saltar
        if (estaAterra && saltar && rb.gravityScale != 0f)
        {
            estaAterra = false;
            rb.AddForce(new Vector2(0f, forcaSalt));
        }
    }

    /// <summary>
    /// Gira el sprite del jugador horitzontalment invertint l'escala X.
    /// S'utilitza per orientar el jugador segons la direcció de moviment.
    /// </summary>
    private void Girar()
    {
        // Girar el jugador
        miraDreta = !miraDreta;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }

    /// <summary>
    /// Detecta quan el jugador entra en contacte amb una escala.
    /// Guarda la referència al centre de l'escala per centrar el jugador.
    /// </summary>
    /// <param name="other">El collider amb el qual ha col·lisionat.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Si el jugador entra en contacte amb una escala
        if (other.CompareTag("Escales"))
        {
            escales = true;
            centreEscales = other.transform.Find("CentreEscales");
        }
    }

    /// <summary>
    /// Detecta quan el jugador surt del contacte amb una escala.
    /// Restaura la gravetat i neteja la referència al centre de l'escala.
    /// </summary>
    /// <param name="other">El collider del qual s'ha separat.</param>
    private void OnTriggerExit2D(Collider2D other)
    {
        // Si el jugador surt de l'escala
        if (other.CompareTag("Escales"))
        {
            escales = false;
            rb.gravityScale = gravetat;
            centreEscales = null;
        }
    }

    /// <summary>
    /// Dibuixa gizmos a l'editor per visualitzar el controlador de terra i el collider del jugador.
    /// El controlador de terra es mostra en vermell, el collider en blau.
    /// </summary>
    private void OnDrawGizmos()
    {
        // Dibuixar el controlador de terra
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(controladorTerra.position, midaCaixaControlador);

        // Obté el collider (si existeix)
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        if (box == null) return;

        // Color del gizmo (groc)
        Gizmos.color = Color.blue;

        // Calcular posició i mides amb offset i escala
        Vector3 pos = box.transform.TransformPoint(box.offset);
        Vector3 size = Vector3.Scale(box.size, box.transform.lossyScale);

        // Dibuixa el rectangle
        Gizmos.DrawWireCube(pos, size);
    }

}
