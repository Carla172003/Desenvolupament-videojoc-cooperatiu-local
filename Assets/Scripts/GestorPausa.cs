using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gestor global que detecta la tecla Escape per pausar/reprendre el joc.
/// Aquest script ha d'estar en un GameObject que estigui sempre actiu.
/// </summary>
public class GestorPausa : MonoBehaviour
{
    [SerializeField] private ControladorPausa controladorPausa; // Referència al ControladorPausa
    private ControladorPanellsInfo controladorPanellsInfo; // Referència als panells informatius

    /// <summary>
    /// Cerca automàticament el ControladorPausa si no s'ha assignat.
    /// </summary>
    void Start()
    {
        if (controladorPausa == null)
        {
            controladorPausa = FindObjectOfType<ControladorPausa>();
        }
        
        controladorPanellsInfo = FindObjectOfType<ControladorPanellsInfo>();
    }

    /// <summary>
    /// Comprova si es prem la tecla Escape per obrir/tancar el menú de pausa.
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // No permetre pausar si els panells informatius estan actius
            if (EsPanellsInfoActius())
            {
                return;
            }

            if (controladorPausa != null)
            {
                // Comprova si el panell està actiu
                if (Time.timeScale == 0f)
                {
                    controladorPausa.ReprendreJoc();
                }
                else
                {
                    controladorPausa.PausarJoc();
                }
            }
        }
    }

    /// <summary>
    /// Comprova si hi ha panells informatius actius.
    /// </summary>
    /// <returns>True si hi ha panells informatius actius, false altrament.</returns>
    private bool EsPanellsInfoActius()
    {
        if (controladorPanellsInfo == null)
        {
            return false;
        }

        // Comprova si algun dels panells informatius està actiu
        if (controladorPanellsInfo.panells != null)
        {
            foreach (GameObject panell in controladorPanellsInfo.panells)
            {
                if (panell != null && panell.activeSelf)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
