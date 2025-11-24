using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ControladorEscena : MonoBehaviour
{

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
}
