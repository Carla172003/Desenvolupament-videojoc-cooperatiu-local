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

    // Suma punts a la puntuació actual
    public void SumarPunts(int punts)
    {
        
        puntuacio += punts;
        puntuacioUI.text = puntuacio + " pts";  
    }

    // Calcula la puntuació final
    public int CalcularPuntuacioFinal(float tempsRestant)
    {
        return puntuacio + Mathf.RoundToInt(tempsRestant);
    }
    
    // Calcula les estrelles obtingudes
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
