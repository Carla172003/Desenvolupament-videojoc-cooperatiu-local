using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ControladorFocus : MonoBehaviour
{
    private Light2D spotlight;

    public Color colorActual;

    [Header("Colores posibles")]
    public Color[] colorsDisponibles;

    /// <summary>
    /// Se llama SOLO cuando el focus se coloca en un punto con luz
    /// </summary>
    public void ConfigurarDesdePunt(PuntColocacio punt)
    {
        // Si este punto no usa luz, no hacemos nada
        if (punt.spotlight == null)
            return;

        spotlight = punt.spotlight;

        spotlight.intensity = 0f;

        if (colorsDisponibles.Length > 0)
            colorActual = colorsDisponibles[0];

        spotlight.color = colorActual;
    }

    public void EncenderLuz()
    {
        if (spotlight == null) return;

        spotlight.intensity = 1f;

        Color c = colorActual;
        c.a = 1f;
        spotlight.color = c;
    }

    public void CambiarColor()
    {
        if (spotlight == null || colorsDisponibles.Length == 0) return;

        int index = System.Array.IndexOf(colorsDisponibles, colorActual);
        index = (index + 1) % colorsDisponibles.Length;

        colorActual = colorsDisponibles[index];
        spotlight.color = colorActual;
    }

    public void CambiarColorAleatorio()
    {
        if (spotlight == null || colorsDisponibles.Length == 0) return;

        int index = Random.Range(0, colorsDisponibles.Length);
        colorActual = colorsDisponibles[index];
        spotlight.color = colorActual;
    }
}
