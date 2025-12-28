using UnityEngine;
using System.Collections;

public class InteraccioNPC : MonoBehaviour
{
    [Header("Diálogo")]
    public SpriteRenderer dialogoSprite;    
    public KeyCode teclaInteraccion = KeyCode.E;
    public float tiempoMostrar = 5f;        

    [Header("ID del NPC")]
    public string idNPC; // Coincide con idObjecte del objeto de vestimenta

    [Header("Referencia al NPC")]
    public ControladorNPC controladorNPC; // Asignar en inspector

    private bool jugadorCerca = false;
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

        if (jugadorCerca && Input.GetKeyDown(teclaInteraccion))
        {
            // Colocar vestimenta si hay objeto correcto
            if (objetoCerca != null && objetoCerca.idObjecte == idNPC && !objetoCerca.colocat)
            {
                //objetoCerca.IntentarColocar();
                Destroy(objetoCerca.gameObject);
                objetoCerca = null;

                // Cambiar animaciones del NPC
                controladorNPC.SetVestidoPuesto(true);
                // Comprobar victoria
                Debug.Log("Intereccio: Comprovant victòria després de vestir NPC " + controladorNPC.name);
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
    }

    private IEnumerator DesactivarDialogo()
    {
        yield return new WaitForSeconds(tiempoMostrar);
        if (dialogoSprite != null)
            dialogoSprite.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Jugador1") || other.CompareTag("Jugador2"))
            jugadorCerca = true;

        ControladorObjecte obj = other.GetComponent<ControladorObjecte>();
        if (obj != null && obj.idObjecte == idNPC && !obj.colocat)
            objetoCerca = obj;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Jugador1") || other.CompareTag("Jugador2"))
        {
            jugadorCerca = false;
            if (dialogoSprite != null)
                dialogoSprite.enabled = false;
        }

        ControladorObjecte obj = other.GetComponent<ControladorObjecte>();
        if (obj == objetoCerca)
            objetoCerca = null;
    }
}
