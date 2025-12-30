using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ControladorEscena : MonoBehaviour
{
    public AudioSource audioSource; 
    public AudioClip clip;

    public void CarregarEscena(string nomEscena)
    {
        SceneManager.LoadScene(nomEscena);
    }
    
    public void SortirJoc()
    {
        Application.Quit();
    }

    public void ReiniciarEscenaNivell()
    {
        Scene escenaActual = SceneManager.GetActiveScene();
        SceneManager.LoadScene(escenaActual.name);
    }

    public void SoCarregarEscena(string nomEscena)
    {
        if (clip != null)
            StartCoroutine(PlayThenLoadCoroutine(clip, nomEscena));
        else
            SceneManager.LoadScene(nomEscena); 
    }

    private IEnumerator PlayThenLoadCoroutine(AudioClip clip, string nomEscena)
    {
        audioSource.PlayOneShot(clip);
        yield return new WaitWhile(() => audioSource.isPlaying);
        SceneManager.LoadScene(nomEscena);
    }
    
}
