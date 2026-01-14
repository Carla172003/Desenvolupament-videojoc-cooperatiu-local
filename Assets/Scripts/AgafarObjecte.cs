using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controlador per agafar i deixar objectes.
/// Permet als jugadors recollir objectes, mantenir-los a ras de terra i deixar-los per col·locar-los.
/// Cada jugador té una tecla especial assignada segons el seu tag (Ma1 o Ma2).
/// </summary>
public class AgafarObjecte : MonoBehaviour
{
    public GameObject puntAgafar; // Mano o punto donde se sujeta
    public float separacioDelSol = 0.05f; // Pequeña separación del suelo
    public float offsetHoritzontal = 0.3f;  // Desplaçament lateral respecte al personatge
    private GameObject objecteAgafat;
    private KeyCode specialKey;
    private Animator animator;
    [SerializeField] public bool teObjecte;
    private GameObject objecteProper;

    [Header("Audio")]
    public AudioClip soAgafar;
    public AudioClip soDeixar;


    /// <summary>
    /// Inicialitza el component. Assigna les tecles segons el tag i obté l'animator del jugador pare.
    /// </summary>
    void Start()
    {
        AssignarTecles();
        animator = GetComponentInParent<Animator>();
    }

    /// <summary>
    /// Processa l'input del jugador cada frame.
    /// Si es prem la tecla especial, agafa o deixa l'objecte.
    /// Manté l'objecte posicionat a ras de terra mentre s'està agafant.
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(specialKey))
        {
            if (teObjecte)
                DeixarObjecte();
            else if (objecteProper != null)
                Agafar(objecteProper);
        }

        // Mantener el objeto a ras de suelo si lo estamos sujetando
        if (teObjecte && objecteAgafat != null)
        {
            PosicionarARasDeSol(objecteAgafat);
        }

        animator.SetBool("teObjecte", teObjecte);
    }

    /// <summary>
    /// Detecta quan un objecte entra dins l'àrea d'agafada del jugador.
    /// Guarda la referència de l'objecte proper si té el tag "Objecte".
    /// </summary>
    /// <param name="other">El collider que ha entrat.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Objecte")) return;

        objecteProper = other.gameObject;
    }

    /// <summary>
    /// Detecta quan un objecte surt de l'àrea d'agafada del jugador.
    /// Neteja la referència de l'objecte proper.
    /// </summary>
    /// <param name="other">El collider que ha sortit.</param>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Objecte")) return;

        if (objecteProper == other.gameObject) objecteProper = null;
    }

    /// <summary>
    /// Agafa un objecte i l'adjunta al punt d'agafada del jugador.
    /// Desactiva la física de l'objecte i ignora les col·lisions amb el jugador.
    /// Reprodueix el so d'agafar si està disponible.
    /// </summary>
    /// <param name="obj">L'objecte a agafar.</param>
    private void Agafar(GameObject obj)
    {
        ControladorObjecte controlador = obj.GetComponent<ControladorObjecte>();
        if (controlador != null && controlador.estaAgafat)
        {
            // Otro jugador ya lo tiene
            return;
        }
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.isKinematic = true;

        obj.transform.SetParent(puntAgafar.transform);
        PosicionarARasDeSol(obj);

        IgnorarColisionsJugador(obj, true);

        objecteAgafat = obj;
        teObjecte = true;

        if (controlador != null)
            controlador.estaAgafat = true;

        ControladorSo.Instance?.ReproduirSoUncop(soAgafar);
    }

    /// <summary>
    /// Deixa l'objecte que s'està agafant. Restaura la física de l'objecte,
    /// reactiva les col·lisions i intenta col·locar-lo en un punt de col·locació si és possible.
    /// Reprodueix el so de deixar si està disponible.
    /// </summary>
    private void DeixarObjecte()
    {
        if (objecteAgafat == null) return;

        Rigidbody2D rb = objecteAgafat.GetComponent<Rigidbody2D>();
        rb.gravityScale = 1;
        rb.isKinematic = false;

        objecteAgafat.transform.SetParent(null);

        IgnorarColisionsJugador(objecteAgafat, false);


        ControladorObjecte colocable = objecteAgafat.GetComponent<ControladorObjecte>();

        if (colocable != null)
        {
            colocable.estaAgafat = false;
            colocable.IntentarColocar();
        }

        ControladorSo.Instance?.ReproduirSoUncop(soDeixar);

        objecteAgafat = null;
        teObjecte = false;
    }

    /// <summary>
    /// Posiciona un objecte a ras de terra al punt d'agafada del jugador.
    /// Calcula l'alçada del collider de l'objecte i ajusta la seva posició Y.
    /// Utilitza posició directa per evitar acumulació de desplaçament.
    /// L'offset horitzontal s'adapta segons la direcció que mira el personatge.
    /// El lateral del collider de l'objecte es posiciona al costat del jugador.
    /// La base del collider de l'objecte està a la mateixa altura que la base del jugador.
    /// </summary>
    /// <param name="obj">L'objecte a posicionar.</param>
    private void PosicionarARasDeSol(GameObject obj)
    {
        Collider2D col = obj.GetComponent<Collider2D>();
        if (col == null) return;

        // Mides del collider de l'objecte
        float altura = col.bounds.size.y;
        float amplada = col.bounds.size.x;

        // Obtenir el collider del jugador per saber l'altura del terra
        Collider2D jugadorCol = transform.parent.GetComponent<Collider2D>();
        float alturaDelTerra = jugadorCol != null ? jugadorCol.bounds.min.y : puntAgafar.transform.position.y;

        // Calcular la posició Y del centre de l'objecte perquè la seva base estigui al terra
        float posicioYDesitjada = alturaDelTerra + (altura / 2f) + separacioDelSol;

        // Determinar la direcció del personatge (localScale.x negatiu = mira esquerra)
        float direccio = transform.parent.localScale.x > 0 ? 1f : -1f;

        // Calcular desplaçament: offset + meitat de l'amplada per col·locar el lateral
        float desplacamentTotal = (offsetHoritzontal + (amplada / 2f)) * direccio;

        // Establir posició directament (no acumulativa)
        Vector3 novaPosicio = obj.transform.position;
        novaPosicio.y = posicioYDesitjada;
        novaPosicio.x = puntAgafar.transform.position.x + desplacamentTotal;
        obj.transform.position = novaPosicio;
    }

    /// <summary>
    /// Assigna la tecla especial per agafar/deixar objectes segons el tag.
    /// Ma1 (Jugador1): E
    /// Ma2 (Jugador2): LeftShift o RightShift
    /// </summary>
    private void AssignarTecles()
    {
        if (CompareTag("Ma1"))
            specialKey = KeyCode.E;
        else if (CompareTag("Ma2"))
            specialKey = KeyCode.RightShift;
    }

    /// <summary>
    /// Dibuixa gizmos a l'editor per visualitzar l'àrea d'agafada d'objectes.
    /// Es mostra en color verd.
    /// </summary>
    private void OnDrawGizmos()
    {
        // Obté el collider (si existeix)
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        if (box == null) return;

        // Color del gizmo (groc)
        Gizmos.color = Color.green;

        // Calcular posició i mides amb offset i escala
        Vector3 pos = box.transform.TransformPoint(box.offset);
        Vector3 size = Vector3.Scale(box.size, box.transform.lossyScale);

        // Dibuixa el rectangle
        Gizmos.DrawWireCube(pos, size);
    }

    /// <summary>
    /// Activa o desactiva les col·lisions entre l'objecte i el jugador.
    /// Utilitzat per evitar que l'objecte empenti el jugador quan l'està agafant.
    /// </summary>
    /// <param name="obj">L'objecte del qual gestionar les col·lisions.</param>
    /// <param name="ignorar">True per ignorar col·lisions, false per restaurar-les.</param>
    private void IgnorarColisionsJugador(GameObject obj, bool ignorar)
    {
        Collider2D objCol = obj.GetComponent<Collider2D>();
        if (objCol == null) return;

        Collider2D[] playerCols = GetComponentsInParent<Collider2D>();

        foreach (var col in playerCols)
        {
            if (!col.isTrigger)
            {
                Physics2D.IgnoreCollision(objCol, col, ignorar);
            }
        }
    }

}
