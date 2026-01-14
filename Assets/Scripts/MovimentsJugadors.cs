using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controlador de moviments dels jugadors.
/// Gestiona el moviment horitzontal, salt, interacció amb escales i animacions.
/// Suporta dos jugadors amb tecles diferents segons el tag (Jugador1 o Jugador2).
/// Utilitza el patró Estat per gestionar els diferents tipus de moviment.
/// </summary>
public class MovimentsJugadors : MonoBehaviour
{
    // Components
    public Rigidbody2D rb { get; private set; }
    public Animator animator { get; private set; }
    public AgafarObjecte agafarObjecte { get; private set; }

    // Estat actual del jugador
    private EstatJugador estatActual;

    [SerializeField] private bool socJugador1;

    [Header("Moviment Horizontal Jugador")]
    public float movimentHorizontal = 0f;
    [SerializeField] public float velocitatMoviment;
    [Range(0f, 0.3f)] [SerializeField] private float suavitzat;

    private Vector3 velocitat = Vector3.zero;
    private bool miraDreta = true;

    [Header("Salt Jugador")]
    [SerializeField] public float forcaSalt;
    [SerializeField] private LayerMask queEsTerra;
    [SerializeField] private Transform controladorTerra;
    [SerializeField] private Vector3 midaCaixaControlador;
    public bool estaAterra { get; set; }
    public float gravetat { get; private set; } = 1.5f;

    // Escales
    public bool escales { get; set; } = false;
    [SerializeField] public float velocitatEscales;
    public Transform centreEscales { get; set; }

    // Tecles
    public KeyCode leftKey { get; private set; }
    public KeyCode rightKey { get; private set; }
    public KeyCode upKey { get; private set; }
    public KeyCode downKey { get; private set; }

    /// <summary>
    /// Inicialitza els components i assigna les tecles segons el jugador.
    /// Determina si és Jugador1 o Jugador2 segons el tag de l'objecte.
    /// Inicialitza l'estat per defecte (EstadoNormal).
    /// </summary>
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.gravityScale = gravetat;
        socJugador1 = CompareTag("Jugador1");
        assignarTecles();
        agafarObjecte = GetComponentInChildren<AgafarObjecte>();

        // Inicialitzar estat per defecte
        estatActual = new EstatCaminant(this);
    }

    /// <summary>
    /// Processa l'input del jugador cada frame utilitzant l'estat actual.
    /// </summary>
    void Update()
    {
        // Processar input segons l'estat actual
        estatActual.ProcessarInput();

        // Comprovar si cal canviar d'estat
        EstatJugador nouEstat = estatActual.ComprovarTransicions();
        if (nouEstat != null)
        {
            estatActual = nouEstat;
        }
    }

    /// <summary>
    /// Aplica el moviment físic del jugador cada frame fixat utilitzant l'estat actual.
    /// </summary>
    private void FixedUpdate()
    {
        estatActual.ActualitzarFisica();
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
    /// Comprova si el jugador està en contacte amb terra, objectes o l'altre jugador.
    /// Utilitza Physics2D.OverlapBoxAll per detectar col·lisions a la base del jugador.
    /// Els jugadors poden estar a sobre l'un de l'altre.
    /// Mètode públic cridat pels estats.
    /// </summary>
    public void ComprovarEstaAterra()
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
    /// Mètode públic cridat pels estats.
    /// </summary>
    /// <param name="moure">Quantitat de moviment horitzontal a aplicar.</param>
    /// <param name="saltar">Si el jugador ha de saltar en aquest frame.</param>
    public void ExecutarMoviment(float moure, bool saltar)
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
