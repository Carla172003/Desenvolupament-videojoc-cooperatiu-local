using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Controlador de la puntuació del joc.
/// </summary>
public class ControladorPuntuacio : MonoBehaviour
{
    public static ControladorPuntuacio Instance;

    [SerializeField] private TextMeshProUGUI puntuacioUI;
    public int puntuacio = 0;
    
    [Header("Llindars d'estrelles")]
    [SerializeField] private int puntuacioMin2Estrelles = 9600;
    [SerializeField] private int puntuacioMin3Estrelles = 12600;
    
    public int numEstrelles { get; private set; } = 0;

    void Awake()
    {
        Instance = this;
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
    /// Calcula el nombre d'estrelles obtingudes segons la puntuació final.
    /// Si partida no completada: 0 estrelles.
    /// Si completada: compara amb els llindars definits.
    /// </summary>
    /// <param name="puntuacioFinal">Puntuació final de la partida.</param>
    /// <param name="partidaCompletada">True si la partida s'ha completat amb èxit.</param>
    /// <returns>Nombre d'estrelles (0-3).</returns>
    public int CalcularEstrelles(int puntuacioFinal, bool partidaCompletada)
    {
        // 0 estrelles si la partida no s'ha completat
        if (!partidaCompletada)
        {
            numEstrelles = 0;
            return numEstrelles;
        }

        // Determinar estrelles segons els llindars configurats
        if (puntuacioFinal >= puntuacioMin3Estrelles)
        {
            numEstrelles = 3;
        }
        else if (puntuacioFinal >= puntuacioMin2Estrelles)
        {
            numEstrelles = 2;
        }
        else
        {
            numEstrelles = 1;
        }
        
        return numEstrelles;
    }
}
