using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controlador del menú de pausa del joc.
/// Permet pausar i reprendre la partida ajustant Time.timeScale.
/// </summary>
public class ControladorPausa : MonoBehaviour
{

    /// <summary>
    /// Assegura que el menú de pausa estigui desactivat al començar.
    /// </summary>
    void Start()
    {
        gameObject.SetActive(false); // Assegura que el menú de pausa estigui desactivat al començar
    }
    
    /// <summary>
    /// Pausa el joc establint Time.timeScale a 0 i activa el menú de pausa.
    /// </summary>
    public void PausarJoc()
    {
        Time.timeScale = 0f; // Pausa el joc
        gameObject.SetActive(true);  
    }

    /// <summary>
    /// Reprendre el joc establint Time.timeScale a 1 i desactiva el menú de pausa.
    /// </summary>
    public void ReprendreJoc()
    {
        Time.timeScale = 1f; // Reanuda el joc
        gameObject.SetActive(false);
    }
    
}
