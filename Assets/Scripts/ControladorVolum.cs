using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI; // Para poder usar Slider

public class ControladorMusicaSo : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider musicSlider;
    public Slider sfxSlider;

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

    public void SetMusicVolume(float value)
    {
        audioMixer.SetFloat("VolumeMusica", value);
    }

    public void SetSFXVolume(float value)
    {
        audioMixer.SetFloat("VolumeSo", value);
    }
}
