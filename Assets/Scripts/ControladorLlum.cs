using UnityEngine;
using UnityEngine.Rendering.Universal; // Necesario para Light2D

/// <summary>
/// Controlador d'efecte de parpelleig per a llums de 2D.
/// Simula l'efecte d'una espelma variant la intensitat i el radi del llum.
/// </summary>
public class ControladorLlum : MonoBehaviour
{
    public Light2D candleLight;

    [Header("Intensidad")]
    public float minIntensity = 0.8f;
    public float maxIntensity = 1.2f;

    [Header("Radio (Opcional)")]
    public float minRadius = 1.8f;
    public float maxRadius = 3.5f;

    [Header("Velocidad del parpadeo")]
    public float flickerSpeed = 1f;

    /// <summary>
    /// Actualitza la intensitat i el radi del llum cada frame utilitzant Perlin Noise
    /// per aconseguir un efecte de parpelleig natural.
    /// </summary>
    void Update()
    {
        // Usamos Perlin Noise para suavidad natural
        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0f);

        // Cambiar intensidad
        candleLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);

        // Cambiar radio (si quieres que "respire")
        candleLight.pointLightOuterRadius = Mathf.Lerp(minRadius, maxRadius, noise);
    }
}
