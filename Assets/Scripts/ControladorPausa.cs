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
    /// Pausa el joc utilitzant el GameManager i activa el menú de pausa.
    /// </summary>
    public void PausarJoc()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.Pausar();
        }
        gameObject.SetActive(true);  
    }

    /// <summary>
    /// Reprendre el joc utilitzant el GameManager i desactiva el menú de pausa.
    /// </summary>
    public void ReprendreJoc()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.Reprendre();
        }
        gameObject.SetActive(false);
    }
    
}
