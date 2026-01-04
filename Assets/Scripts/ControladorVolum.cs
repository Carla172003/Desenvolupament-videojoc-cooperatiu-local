using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI; // Para poder usar Slider

/// <summary>
/// Controlador dels sliders de volum de música i efectes de so.
/// Utilitza un AudioMixer per controlar els volums de manera independent.
/// </summary>
public class ControladorMusicaSo : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider musicSlider;
    public Slider sfxSlider;

    /// <summary>
    /// Inicialitza els sliders amb els valors actuals de l'AudioMixer.
    /// </summary>
    private void Start()
    {
        // Inicializa sliders con los valores actuales del AudioMixer
        float musicValue;
        audioMixer.GetFloat("VolumeMusica", out musicValue);
        musicSlider.value = musicValue;

        float sfxValue;
        audioMixer.GetFloat("VolumeSo", out sfxValue);
        sfxSlider.value = sfxValue;

        // Suscribimos eventos de cambio
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    /// <summary>
    /// Estableix el volum de la música a l'AudioMixer.
    /// </summary>
    /// <param name="value">Valor del volum.</param>
    public void SetMusicVolume(float value)
    {
        audioMixer.SetFloat("VolumeMusica", value);
    }

    /// <summary>
    /// Estableix el volum dels efectes de so a l'AudioMixer.
    /// </summary>
    /// <param name="value">Valor del volum.</param>
    public void SetSFXVolume(float value)
    {
        audioMixer.SetFloat("VolumeSo", value);
    }
}
