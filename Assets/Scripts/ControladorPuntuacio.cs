using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Controlador de la puntuació del joc.
/// Utilitza el patró Strategy per al càlcul d'estrelles.
/// </summary>
public class ControladorPuntuacio : MonoBehaviour
{
    public static ControladorPuntuacio Instance;

    [SerializeField] private TextMeshProUGUI puntuacioUI;
    public int puntuacio = 0;
    
    [Header("Llindars d'estrelles")]
    [SerializeField] private int puntuacioMin2Estrelles = 9600;
    [SerializeField] private int puntuacioMin3Estrelles = 12600;
    
    // Estratègia per calcular estrelles (patró Strategy)
    private ISistemaPuntuacio estrategiaEstrelles;
    
    public int numEstrelles { get; private set; } = 0;

    void Awake()
    {
        Instance = this;
        
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
            // Reiniciar puntuació i UI al començar un nivell
            puntuacio = 0;
            if (puntuacioUI != null)
            {
                puntuacioUI.text = "0 pts";
            }
        }
    }

    /// <summary>
    /// Suma punts a la puntuació actual i actualitza la UI.
    /// </summary>
    /// <param name="punts">Quantitat de punts a sumar.</param>
    public void SumarPunts(int punts)
    {
        puntuacio += punts;
        puntuacioUI.text = puntuacio + " pts";  
    }

    /// <summary>
    /// Calcula la puntuació final sumant el temps restant a la puntuació actual.
    /// </summary>
    /// <param name="tempsRestant">Temps restant en segons.</param>
    /// <returns>Puntuació final calculada.</returns>
    public int CalcularPuntuacioFinal(float tempsRestant)
    {
        int puntuacioFinal = puntuacio + Mathf.RoundToInt(tempsRestant);
        return puntuacioFinal;
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
}
