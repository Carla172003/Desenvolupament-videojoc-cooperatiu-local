using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Zona que permet generar objectes aleatoris quan els jugadors interaccionen.
/// Cada objecte es pot generar una sola vegada i s'elimina de la llista després.
/// Controla que només hi hagi un objecte a la zona alhora.
/// </summary>
public class ZonaObjectes : MonoBehaviour
{
    public enum TipusArmari
    {
        Cap,
        Attrezzo,
        Llums,
        Vestimenta
    }

    [Header("Objectes possibles (prefabs)")]
    public List<GameObject> objectesPossibles; 

    [Header("Punt on es instancien")]
    public Transform puntSpawn;

    [Header("Tipus d'armari")]
    public TipusArmari tipusArmari = TipusArmari.Cap;
    private Animator animator;

    private KeyCode specialKeyJugador1 = KeyCode.E;
    private KeyCode specialKeyJugador2 = KeyCode.RightShift;

    private bool jugador1Dins = false;
    private bool jugador2Dins = false;

    private bool hayObjetoDentro = false; // true si ya hay un objeto en la zona o el jugador entra con uno

    /// <summary>
    /// Inicialitza el component. Obté l'animator si és un armari.
    /// </summary>
    void Start()
    {
        if (tipusArmari != TipusArmari.Cap)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("[ZonaObjectes] No s'ha trobat Animator al GameObject " + gameObject.name);
            }
            else
            {
                Debug.Log("[ZonaObjectes] Animator trobat correctament al GameObject " + gameObject.name);
            }
        }
    }

    /// <summary>
    /// Comprova si algun jugador dins de la zona prem la tecla d'interacció.
    /// Només genera objectes si no n'hi ha cap a la zona.
    /// </summary>
    void Update()
    {
        // Solo generar si no hay objeto dentro
        if (!hayObjetoDentro)
        {
            if (jugador1Dins && Input.GetKeyDown(specialKeyJugador1))
            {
                GenerarObjecteRandom();
            }

            if (jugador2Dins && Input.GetKeyDown(specialKeyJugador2))
            {
                GenerarObjecteRandom();
            }
        }
    }

    /// <summary>
    /// Genera un objecte aleatori de la llista d'objectes possibles.
    /// L'objecte es crea al punt de spawn i s'elimina de la llista per evitar repeticions.
    /// Si és un armari d'attrezzo i es queda sense objectes, activa l'animació de tancar portes.
    /// </summary>
    private void GenerarObjecteRandom()
    {
        if (objectesPossibles == null || objectesPossibles.Count == 0)
        {
            Debug.Log("[ZonaObjectes] No hi ha objectes disponibles a la llista");
            return;
        }

        int index = Random.Range(0, objectesPossibles.Count);
        GameObject prefab = objectesPossibles[index];

        // Crear nuevo objeto en el punto de spawn
        GameObject nuevoObjeto = Instantiate(prefab, puntSpawn.position, Quaternion.identity);
        nuevoObjeto.name = prefab.name;

        // Marcar que ahora hay objeto dentro
        hayObjetoDentro = true;

        // Eliminar prefab de la lista para no repetir
        objectesPossibles.RemoveAt(index);
        
        Debug.Log("[ZonaObjectes] Objecte generat: " + prefab.name + ". Objectes restants: " + objectesPossibles.Count);

        // Si és un armari i ja no queden més objectes, tancar portes
        if (tipusArmari != TipusArmari.Cap && objectesPossibles.Count == 0)
        {
            Debug.Log("[ZonaObjectes] No queden més objectes! Tancant portes de l'armari...");
            
            if (animator != null)
            {
                string triggerName = ObtenirNomTrigger();
                Debug.Log("[ZonaObjectes] Activant trigger '" + triggerName + "'");
                animator.SetTrigger(triggerName);
            }
            else
            {
                Debug.LogError("[ZonaObjectes] Animator és null! No es pot tancar l'armari.");
            }
        }
    }

    /// <summary>
    /// Obté el nom del trigger d'animació segons el tipus d'armari.
    /// </summary>
    private string ObtenirNomTrigger()
    {
        switch (tipusArmari)
        {
            case TipusArmari.Attrezzo:
                return "TancarPortesArmari";
            case TipusArmari.Llums:
                return "TancarPortesLlums";
            case TipusArmari.Vestimenta:
                return "TancarPortesVestimenta";
            default:
                return "";
        }
    }

    /// <summary>
    /// Detecta quan un jugador o objecte entra a la zona.
    /// Marca que hi ha un objecte dins si el collider pertany a un objecte col·locable.
    /// </summary>
    /// <param name="other">El collider que ha entrat.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Jugador1"))
            jugador1Dins = true;
        else if (other.CompareTag("Jugador2"))
            jugador2Dins = true;

        // Si el jugador entra con un objeto (por ejemplo un objeto que tenga un componente ControladorObjecte)
        if (other.GetComponent<ControladorObjecte>() != null)
            hayObjetoDentro = true;
    }

    /// <summary>
    /// Detecta quan un jugador o objecte surt de la zona.
    /// Si un objecte surt, permet generar-ne un altre.
    /// </summary>
    /// <param name="other">El collider que ha sortit.</param>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Jugador1"))
            jugador1Dins = false;
        else if (other.CompareTag("Jugador2"))
            jugador2Dins = false;

        // Si el objeto sale de la zona, permitimos generar otro
        if (other.GetComponent<ControladorObjecte>() != null)
            hayObjetoDentro = false;
    }
}
