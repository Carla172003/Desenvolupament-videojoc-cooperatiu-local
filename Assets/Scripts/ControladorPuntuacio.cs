using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Controlador de la puntuació del joc.
/// Utilitza el patró Strategy per al càlcul d'estrelles.
/// Implementa el patró Observer per actualitzar la UI quan canvia la puntuació.
/// </summary>
public class ControladorPuntuacio : MonoBehaviour, IObservadorPuntuacio
{
    public static ControladorPuntuacio Instance;

    [SerializeField] private TextMeshProUGUI puntuacioUI;
    
    [Header("Llindars d'estrelles")]
    [SerializeField] private int puntuacioMin2Estrelles = 9600;
    [SerializeField] private int puntuacioMin3Estrelles = 12600;
    
    // Model de domini que gestiona la puntuació (patró Observer)
    private ModelPuntuacio modelPuntuacio;
    
    // Estratègia per calcular estrelles (patró Strategy)
    private ISistemaPuntuacio estrategiaEstrelles;
    
    // Propietat per mantenir compatibilitat amb codi existent
    public int puntuacio 
    { 
        get => modelPuntuacio?.ObtenirPuntuacio() ?? 0;
    }
    
    public int numEstrelles { get; private set; } = 0;

    void Awake()
    {
        Instance = this;
        
        // Crear model de puntuació i subscriure's com a observador
        modelPuntuacio = new ModelPuntuacio();
        modelPuntuacio.SubscriureObservador(this);
        
        // Inicialitzar estratègia de càlcul d'estrelles
        estrategiaEstrelles = new Puntuacio3Estrelles(
            puntuacioMin2Estrelles, 
            puntuacioMin3Estrelles
        );
    }

    void Start()
    {
        // Només reiniciar puntuació si estem en un nivell jugable, no a Victoria/Derrota
        string escenaActual = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        
        if (escenaActual.StartsWith("Nivell"))
        {
            // Reiniciar puntuació (notificarà automàticament i actualitzarà la UI)
            modelPuntuacio.ReiniciarPuntuacio();
        }
    }

    /// <summary>
    /// Mètode de l'observador cridat quan la puntuació canvia.
    /// Actualitza automàticament la UI.
    /// </summary>
    /// <param name="novaPuntuacio">La nova puntuació.</param>
    public void ActualitzarPuntuacio(int novaPuntuacio)
    {
        if (puntuacioUI != null)
        {
            puntuacioUI.text = novaPuntuacio + " pts";
        }
    }

    /// <summary>
    /// Suma punts a la puntuació actual.
    /// El model notificarà automàticament i la UI s'actualitzarà.
    /// </summary>
    /// <param name="punts">Quantitat de punts a sumar.</param>
    public void SumarPunts(int punts)
    {
        modelPuntuacio.SumarPunts(punts);
    }

    /// <summary>
    /// Calcula la puntuació final sumant el temps restant a la puntuació actual.
    /// </summary>
    /// <param name="tempsRestant">Temps restant en segons.</param>
    /// <returns>Puntuació final calculada.</returns>
    public int CalcularPuntuacioFinal(float tempsRestant)
    {
        return modelPuntuacio.CalcularPuntuacioFinal(tempsRestant);
    }
    
    /// <summary>
    /// Calcula el nombre d'estrelles obtingudes utilitzant l'estratègia de càlcul (patró Strategy).
    /// </summary>
    /// <param name="puntuacioFinal">Puntuació final de la partida.</param>
    /// <param name="partidaCompletada">True si la partida s'ha completat amb èxit.</param>
    /// <returns>Nombre d'estrelles (0-3).</returns>
    public int CalcularEstrelles(int puntuacioFinal, bool partidaCompletada)
    {
        numEstrelles = estrategiaEstrelles.CalcularEstrelles(puntuacioFinal, partidaCompletada);
        return numEstrelles;
    }
    
    /// <summary>
    /// Canvia l'estratègia de càlcul d'estrelles.
    /// </summary>
    /// <param name="novaEstrategia">Nova estratègia a utilitzar.</param>
    public void CanviarEstrategiaEstrelles(ISistemaPuntuacio novaEstrategia)
    {
        estrategiaEstrelles = novaEstrategia;
    }

    /// <summary>
    /// Desubscriure's de l'observador quan es destrueix el controlador.
    /// </summary>
    void OnDestroy()
    {
        if (modelPuntuacio != null)
        {
            modelPuntuacio.DesubscriureObservador(this);
        }
    }
}
