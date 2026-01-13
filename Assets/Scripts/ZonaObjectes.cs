using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Zona que permet generar objectes aleatoris quan els jugadors interaccionen.
/// Cada objecte es pot generar una sola vegada i s'elimina de la llista després.
/// Controla que només hi hagi un objecte a la zona alhora.
/// Pot estar bloquejat i requerir una llave per desbloquejar-lo.
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
    
    [Header("Bloqueig")]
    public bool estaBloquejar = false;
    public Sprite spriteCadenat; // Sprite del candado que se muestra cuando está bloqueado
    public string idLlave = "Llave"; // ID de l'objecte que funciona com a llave
    
    private bool estaDesbloquejat = false; // true quan s'ha desbloquejar amb una llave
    private SpriteRenderer spriteRenderer;
    private Sprite spriteOriginal; // Guarda el sprite original de l'armari

    private KeyCode specialKeyJugador1 = KeyCode.E;
    private KeyCode specialKeyJugador2 = KeyCode.RightShift;

    private bool jugador1Dins = false;
    private bool jugador2Dins = false;

    private bool hayObjetoDentro = false; // true si ya hay un objeto en la zona o el jugador entra con uno
    private GameObject llaveDetectada = null; // Registra si hi ha una llave dins de la zona

    /// <summary>
    /// Inicialitza el component. Obté l'animator i el SpriteRenderer si és un armari.
    /// Configura l'estat inicial del candado.
    /// </summary>
    void Start()
    {
        // Obtenir el SpriteRenderer del GameObject
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteOriginal = spriteRenderer.sprite; // Guardar sprite original
        }
        
        // Obtenir animator si existeix
        animator = GetComponent<Animator>();
        
        // Configurar estat inicial del candado
        if (estaBloquejar && !estaDesbloquejat)
        {
            MostrarCadenat();
            
            // Activar paràmetre bool de l'animator
            if (animator != null)
            {
                animator.SetBool("estaBloquejat", true);
            }
            
        }
    }
    
    /// <summary>
    /// Mostra el sprite del candado en l'armari.
    /// </summary>
    private void MostrarCadenat()
    {
        if (spriteRenderer != null && spriteCadenat != null)
        {
            spriteRenderer.sprite = spriteCadenat;
        }
    }
    
    /// <summary>
    /// Restaura el sprite original de l'armari.
    /// </summary>
    private void AmagarCadenat()
    {
        if (spriteRenderer != null && spriteOriginal != null)
        {
            spriteRenderer.sprite = spriteOriginal;
        }
    }

    /// <summary>
    /// Comprova si algun jugador dins de la zona prem la tecla d'interacció.
    /// Només genera objectes si no n'hi ha cap a la zona i si l'armari no està bloquejar.
    /// </summary>
    void Update()
    {
        // Comprovar si està bloquejar i no desbloquejar
        if (estaBloquejar && !estaDesbloquejat)
        {
            // No permet generar objectes si està bloquejar
            return;
        }
        
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
        

        // Si és un armari i ja no queden més objectes, tancar portes
        if (tipusArmari != TipusArmari.Cap && objectesPossibles.Count == 0)
        {
            
            if (animator != null)
            {
                string triggerName = ObtenirNomTrigger();
                animator.SetTrigger(triggerName);
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
    /// Registra si és una llave per verificar-la després.
    /// </summary>
    /// <param name="other">El collider que ha entrat.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Jugador1"))
            jugador1Dins = true;
        else if (other.CompareTag("Jugador2"))
            jugador2Dins = true;

        // Si entra un objecte
        ControladorObjecte controlador = other.GetComponent<ControladorObjecte>();
        if (controlador != null)
        {
            hayObjetoDentro = true;
            
            // Si és una llave, registrar-la per verificar-la després
            if (estaBloquejar && !estaDesbloquejat && controlador.idObjecte == idLlave)
            {
                llaveDetectada = other.gameObject;
            }
        }
    }
    
    /// <summary>
    /// Verifica constantment si la llave ha estat soltada per desbloquejar l'armari.
    /// </summary>
    /// <param name="other">El collider que està dins de la zona.</param>
    private void OnTriggerStay2D(Collider2D other)
    {
        // Si hi ha una llave detectada i encara no hem desbloquejar
        if (llaveDetectada != null && estaBloquejar && !estaDesbloquejat)
        {
            // Verificar si la llave està sent agafada consultant el component ControladorObjecte
            ControladorObjecte controlador = llaveDetectada.GetComponent<ControladorObjecte>();
            
            // Només desbloquejar si la llave NO està sent agafada
            if (controlador != null && !controlador.estaAgafat)
            {
                IntentarDesbloquejar(llaveDetectada);
                llaveDetectada = null; // Netejar referència
            }
        }
    }
    
    /// <summary>
    /// Desbloqueja l'armari quan es deixa una llave a prop.
    /// Destrueix la llave i restaura el sprite original de l'armari.
    /// </summary>
    /// <param name="llave">GameObject de la llave.</param>
    private void IntentarDesbloquejar(GameObject llave)
    {
        
        // Desbloquejar l'armari
        estaDesbloquejat = true;
        
        // Restaurar el sprite original de l'armari
        AmagarCadenat();
        
        // Desactivar paràmetre bool per activar transició a obrirPortes
        if (animator != null)
        {
            animator.SetBool("estaBloquejat", false);
        }
        
        // Destruir la llave
        Destroy(llave);
        
        // Reproduir so si hi ha AudioManager
        if (ControladorSo.Instance != null)
        {
            // Pots afegir un so d'desbloquejar aquí si en tens
        }
        
        hayObjetoDentro = false; // Permetre generar objectes ara
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
        {
            hayObjetoDentro = false;
            
            // Si era la llave detectada, netejar referència
            if (llaveDetectada != null && other.gameObject == llaveDetectada)
            {
                llaveDetectada = null;
            }
        }
    }
}
