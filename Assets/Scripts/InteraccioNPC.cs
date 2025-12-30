using UnityEngine;
using System.Collections;

public class InteraccioNPC : MonoBehaviour
{
    [Header("Diálogo")]
    public SpriteRenderer dialogoSprite;    
    public KeyCode teclaJugador1 = KeyCode.E;
    public KeyCode teclaJugador2 = KeyCode.O;
    public float tiempoMostrar = 5f;        

    [Header("ID del NPC")]
    public string idNPC; // Coincide con idObjecte del objeto de vestimenta

    [Header("Referencia al NPC")]
    public ControladorNPC controladorNPC; // Asignar en inspector

    private bool jugador1Cerca = false;
    private bool jugador2Cerca = false;
    private ControladorObjecte objetoCerca = null;

    void Start()
    {
        if (dialogoSprite != null)
            dialogoSprite.enabled = false;

        if (controladorNPC == null)
            controladorNPC = GetComponentInParent<ControladorNPC>();
    }

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
            Debug.Log("Interacción: Comprobando victoria después de vestir NPC " + controladorNPC.name);
            GameManager.Instance?.ComprovarVictoria();
        }
        else
        {
            // Mostrar diálogo
            if (dialogoSprite != null)
            {
                dialogoSprite.enabled = true;
                StopAllCoroutines();
                StartCoroutine(DesactivarDialogo());
            }
        }
    }

    private IEnumerator DesactivarDialogo()
    {
        yield return new WaitForSeconds(tiempoMostrar);
        if (dialogoSprite != null)
            dialogoSprite.enabled = false;
    }

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
