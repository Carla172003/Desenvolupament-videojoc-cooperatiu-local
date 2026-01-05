using UnityEngine;
using System.Collections;

/// <summary>
/// Gestiona la interacció dels jugadors amb els NPCs.
/// Permet vestir NPCs amb objectes que tinguin l'idObjecte corresponent.
/// Mostra diàlegs si el jugador interacciona sense l'objecte correcte.
/// </summary>
public class InteraccioNPC : MonoBehaviour
{
    [Header("Diálogo")]
    public SpriteRenderer dialogoSprite;    
    public KeyCode teclaJugador1 = KeyCode.E;
    public KeyCode teclaJugador2 = KeyCode.RightShift;
    public float tiempoMostrar = 5f;        

    [Header("ID del NPC")]
    public string idNPC; // Coincide con idObjecte del objeto de vestimenta

    [Header("Referencia al NPC")]
    public ControladorNPC controladorNPC; // Asignar en inspector

    private bool jugador1Cerca = false;
    private bool jugador2Cerca = false;
    private ControladorObjecte objetoCerca = null;

    /// <summary>
    /// Inicialitza el component. Desactiva el sprite de diàleg i obté el controlador del NPC.
    /// </summary>
    void Start()
    {
        if (dialogoSprite != null)
            dialogoSprite.enabled = false;

        if (controladorNPC == null)
            controladorNPC = GetComponentInParent<ControladorNPC>();
    }

    /// <summary>
    /// Comprova cada frame si algun jugador proper prem la tecla d'interacció.
    /// No permet interaccions si el NPC ja està vestit.
    /// </summary>
    void Update()
    {
        if (controladorNPC != null && controladorNPC.estaVestit)
            return;

        // Interacción Jugador 1
        if (jugador1Cerca && Input.GetKeyDown(teclaJugador1))
        {
            Interactuar();
        }

        // Interacción Jugador 2
        if (jugador2Cerca && Input.GetKeyDown(teclaJugador2))
        {
            Interactuar();
        }
    }

    /// <summary>
    /// Executa la interacció amb el NPC.
    /// Si hi ha un objecte correcte proper, vesteix el NPC i comprova la victòria.
    /// Si no, mostra el diàleg temporal.
    /// </summary>
    private void Interactuar()
    {
        // Colocar vestimenta si hay objeto correcto
        if (objetoCerca != null && objetoCerca.idObjecte == idNPC && !objetoCerca.colocat)
        {
            Destroy(objetoCerca.gameObject);
            objetoCerca = null;

            // Cambiar animaciones del NPC
            controladorNPC.SetVestidoPuesto(true);
            // Comprobar victoria
            GameManager.Instance?.ComprovarVictoria();
        }
        else
        {
            Debug.Log("Interacción NPC sin objeto correcto.");
            // Mostrar diálogo
            if (dialogoSprite != null)
            {
                Debug.Log("Mostrar diálogo NPC.");
                dialogoSprite.enabled = true;
                StopAllCoroutines();
                StartCoroutine(DesactivarDialogo());
            }
        }
    }

    /// <summary>
    /// Corutina que desactiva el diàleg després d'un temps establert.
    /// </summary>
    /// <returns>IEnumerator per a la corutina.</returns>
    private IEnumerator DesactivarDialogo()
    {
        yield return new WaitForSeconds(tiempoMostrar);
        if (dialogoSprite != null)
            dialogoSprite.enabled = false;
    }

    /// <summary>
    /// Detecta quan un jugador o objecte entra a l'àrea d'interacció del NPC.
    /// Guarda la referència del jugador i de l'objecte proper si coincideix amb l'ID del NPC.
    /// </summary>
    /// <param name="other">El collider que ha entrat.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Jugador1"))
            jugador1Cerca = true;
        else if (other.CompareTag("Jugador2"))
            jugador2Cerca = true;

        ControladorObjecte obj = other.GetComponent<ControladorObjecte>();
        if (obj != null && obj.idObjecte == idNPC && !obj.colocat)
            objetoCerca = obj;
    }

    /// <summary>
    /// Detecta quan un jugador o objecte surt de l'àrea d'interacció del NPC.
    /// Neteja les referències corresponents.
    /// </summary>
    /// <param name="other">El collider que ha sortit.</param>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Jugador1"))
            jugador1Cerca = false;
        else if (other.CompareTag("Jugador2"))
            jugador2Cerca = false;

        if (dialogoSprite != null && !jugador1Cerca && !jugador2Cerca)
            dialogoSprite.enabled = false;

        ControladorObjecte obj = other.GetComponent<ControladorObjecte>();
        if (obj == objetoCerca)
            objetoCerca = null;
    }
}
