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
        return puntuacio + Mathf.RoundToInt(tempsRestant);
    }
    
    /// <summary>
    /// Calcula el nombre d'estrelles obtingudes segons la puntuació i el temps.
    /// Retorna 0 estrelles si s'acaba el temps.
    /// </summary>
    /// <param name="objectesColocats">Nombre d'objectes col·locats correctament.</param>
    /// <param name="totalObjectes">Total d'objectes del nivell.</param>
    /// <param name="tempsRestant">Temps restant en segons.</param>
    /// <param name="timeOut">True si s'ha acabat el temps.</param>
    /// <param name="puntsPerObjecte">Punts atorgats per cada objecte (per defecte 100).</param>
    /// <returns>Nombre d'estrelles (0-3).</returns>
    public int CalcularEstrelles(
        int objectesColocats,
        int totalObjectes,
        float tempsRestant,
        bool timeOut,
        int puntsPerObjecte = 100)
    {
        // 0 ESTRELLAS si se acaba el tiempo
        if (timeOut) return 0;

        // Puntuación final
        float puntuacion = (objectesColocats * puntsPerObjecte) + tempsRestant;

        // Umbrales recomendados
        float p1 = puntsPerObjecte * (totalObjectes / 2f);
        float p2 = puntsPerObjecte * totalObjectes;
        // Determinar estrellas
        if (puntuacion >= p2) return 3;
        if (puntuacion >= p1) return 2;
        return 1;
    }
}
