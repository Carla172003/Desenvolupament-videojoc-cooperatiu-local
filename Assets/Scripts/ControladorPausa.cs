using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControladorPausa : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(false); // Assegura que el menú de pausa estigui desactivat al començar
    }
    
    public void PausarJoc()
    {
        Time.timeScale = 0f; // Pausa el joc
        gameObject.SetActive(true);
    }

    public void ReprendreJoc()
    {
        Time.timeScale = 1f; // Reanuda el joc
        gameObject.SetActive(false);
    }
}
