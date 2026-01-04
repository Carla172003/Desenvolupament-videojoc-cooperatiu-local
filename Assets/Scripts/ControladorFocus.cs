using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Controlador dels objectes focus (llums) que es poden col·locar.
/// Quan un focus es col·loca en un PuntColocacio amb spotlight,
/// s'encarrega d'encendre el llum amb el color adequat.
/// </summary>
public class ControladorFocus : MonoBehaviour
{
    private Light2D spotlight;

    public Color colorActual;

    [Header("Colores posibles")]
    public Color[] colorsDisponibles;

    /// <summary>
    /// Configura el focus quan es col·loca en un punt amb llum.
    /// Assigna el spotlight del punt i estableix el color inicial.
    /// Aquest mètode només es crida quan el punt té un spotlight.
    /// </summary>
    /// <param name="punt">El punt de col·locació amb el spotlight.</param>
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

    /// <summary>
    /// Encen el llum del focus establint la intensitat a 1.
    /// Aplica el color actual al spotlight.
    /// </summary>
    public void EncenderLuz()
    {
        if (spotlight == null) return;

        spotlight.intensity = 1f;

        Color c = colorActual;
        c.a = 1f;
        spotlight.color = c;
    }
}
