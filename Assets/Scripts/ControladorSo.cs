using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

/// <summary>
/// Gestor d'audio del joc. Implementa el patró Singleton per gestionar
/// els efectes de so de manera centralitzada.
/// </summary>
public class ControladorSo : MonoBehaviour
{
    public static ControladorSo Instance;

    public AudioSource efectosSource; 

    public AudioClip clipVictoria;
    public AudioClip clipDerrota;

    /// <summary>
    /// Inicialitza la instància del singleton.
    /// Si ja existeix una instància, destrueix aquest objecte.
    /// </summary>
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Reprodueix un efecte de so una sola vegada.
    /// Utilitzar per a efectes puntuals com salts, agafar objectes, etc.
    /// </summary>
    /// <param name="clip">El clip d'audio a reproduir.</param>
    public void ReproduirSoUncop(AudioClip clip)
    {
        if (clip != null && efectosSource != null)
        {
            efectosSource.PlayOneShot(clip);
        }
    }
    /// <summary>
    /// Reprodueix un efecte de so en bucle continu.
    /// Utilitzar per a sons persistents com el moviment de plataformes.
    /// Cal aturar-lo manualment amb AturarSo().
    /// </summary>
    /// <param name="clip">El clip d'audio a reproduir en bucle.</param>
    public void ReproduirSoEnBucle(AudioClip clip)
    {
        if (clip != null && efectosSource != null)
        {
            efectosSource.clip = clip; 
            efectosSource.loop = true;  
            efectosSource.Play();
        }
    }
    /// <summary>
    /// Atura la reproducció del so en bucle si s'està reproduint.
    /// Utilitzar per aturar sons iniciats amb ReproduirSoEnBucle().
    /// </summary>
    public void AturarSo()
    {
        if (efectosSource != null && efectosSource.isPlaying)
        {
            efectosSource.Stop();
        }
    }

}
