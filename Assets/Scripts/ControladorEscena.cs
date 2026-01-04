using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// Controlador de la gestió d'escenes del joc.
/// Proporciona mètodes per carregar escenes, reiniciar nivells i sortir de l'aplicació.
/// Pot reproduir sons abans de carregar una escena.
/// </summary>
public class ControladorEscena : MonoBehaviour
{
    public AudioSource audioSource; 
    public AudioClip clip;

    /// <summary>
    /// Carrega una escena pel seu nom.
    /// </summary>
    /// <param name="nomEscena">El nom de l'escena a carregar.</param>
    public void CarregarEscena(string nomEscena)
    {
        SceneManager.LoadScene(nomEscena);
    }
    
    /// <summary>
    /// Tanca l'aplicació. Només funciona en builds, no a l'editor.
    /// </summary>
    public void SortirJoc()
    {
        Application.Quit();
    }

    /// <summary>
    /// Reinicia l'escena actual recarregant-la.
    /// Utilitzat per tornar a provar un nivell.
    /// </summary>
    public void ReiniciarEscenaNivell()
    {
        Scene escenaActual = SceneManager.GetActiveScene();
        SceneManager.LoadScene(escenaActual.name);
    }

    /// <summary>
    /// Carrega una escena després de reproduir un clip d'audio.
    /// Espera que el clip acabi abans de carregar l'escena.
    /// </summary>
    /// <param name="nomEscena">El nom de l'escena a carregar.</param>
    public void SoCarregarEscena(string nomEscena)
    {
        if (clip != null)
            StartCoroutine(PlayThenLoadCoroutine(clip, nomEscena));
        else
            SceneManager.LoadScene(nomEscena); 
    }

    /// <summary>
    /// Corutina que reprodueix un so i després carrega una escena.
    /// </summary>
    /// <param name="clip">El clip d'audio a reproduir.</param>
    /// <param name="nomEscena">El nom de l'escena a carregar.</param>
    /// <returns>IEnumerator per a la corutina.</returns>
    private IEnumerator PlayThenLoadCoroutine(AudioClip clip, string nomEscena)
    {
        audioSource.PlayOneShot(clip);
        yield return new WaitWhile(() => audioSource.isPlaying);
        SceneManager.LoadScene(nomEscena);
    }
    
}
