using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PuntColocacio : MonoBehaviour
{
    public string idCorrecte; 
    public bool ocupat = false;

    [Header("Opcional solo para focus")]
    public Light2D spotlight;
    public Color colorCorrecto;
    public Color[] colorsDisponibles; // Colores posibles para el botón

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

    public void CambiarColorAleatorio()
    {
        if (spotlight == null || colorsDisponibles == null || colorsDisponibles.Length == 0)
            return;

        int index = Random.Range(0, colorsDisponibles.Length);
        Color nuevoColor = colorsDisponibles[index];

        spotlight.color = nuevoColor;
    }
}
