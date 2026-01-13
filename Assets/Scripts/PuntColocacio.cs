using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Representa un punt on es poden col·locar objectes.
/// Cada punt té un ID correcte que ha de coincidir amb l'idObjecte.
/// Opcionalment pot tenir un spotlight amb validació de color per als puzzles de llum.
/// </summary>
public class PuntColocacio : MonoBehaviour
{
    public string idCorrecte; 
    public bool ocupat = false;

    [Header("Opcional solo para focus")]
    public Light2D spotlight;
    public Color colorCorrecto;
    public Color[] colorsDisponibles; // Colores posibles para el botón

    /// <summary>
    /// Inicialitza el punt de col·locació. Si té un spotlight, el desactiva i configura l'alpha.
    /// </summary>
    private void Start()
    {
        if (spotlight != null)
        {
            spotlight.intensity = 0f;
            // Forzar alpha a 1 para que cambie luego correctamente
            Color c = spotlight.color;
            c.a = 1f;
            spotlight.color = c;
        }
    }

    /// <summary>
    /// Canvia el color del spotlight a un color aleatori dels disponibles.
    /// Utilitzat pels botons de color per canviar l'estat dels puzzles de llum.
    /// Després comprova la victòria.
    /// </summary>
    public void CambiarColorAleatorio()
    {
        if (spotlight == null || colorsDisponibles == null || colorsDisponibles.Length == 0)
            return;

        int index = Random.Range(0, colorsDisponibles.Length);
        Color nuevoColor = colorsDisponibles[index];

        spotlight.color = nuevoColor;
        GameManager.Instance?.ComprovarVictoria();
        
    }

    /// <summary>
    /// Comprova si el color actual del spotlight coincideix amb el color correcte.
    /// Utilitza una tolerància per comparar els valors RGB.
    /// Si no hi ha spotlight, retorna sempre true.
    /// </summary>
    /// <returns>True si el color és correcte o no hi ha spotlight.</returns>
    public bool ColorEsCorrecto()
    {
        if (spotlight == null)
            return true;

        Color a = spotlight.color;
        Color b = colorCorrecto;

        float tolerancia = 0.01f;

        bool correcto =
            Mathf.Abs(a.r - b.r) < tolerancia &&
            Mathf.Abs(a.g - b.g) < tolerancia &&
            Mathf.Abs(a.b - b.b) < tolerancia;

        return correcto;
    }


}
