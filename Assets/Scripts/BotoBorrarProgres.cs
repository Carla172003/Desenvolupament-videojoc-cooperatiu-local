using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Botó per esborrar tot el progrés guardat dels nivells.
/// Útil per testing i durant el desenvolupament.
/// </summary>
public class BotoBorrarProgres : MonoBehaviour
{
    /// <summary>
    /// Esborra tot el progrés dels nivells i actualitza la UI.
    /// Aquest mètode es pot cridar des d'un botó UI amb onClick.
    /// </summary>
    public void BorrarTotElProgres()
    {
        if (GestorDadesNivells.Instance != null)
        {
            GestorDadesNivells.Instance.EsborrarTotsElsNivells();
            
            // Actualitzar tots els botons de nivell si existeixen
            BotoNivell[] botonsNivell = FindObjectsOfType<BotoNivell>();
            foreach (BotoNivell boto in botonsNivell)
            {
                boto.ActualitzarEstrelles();
            }
            
            Debug.Log("Progrés esborrat i UI actualitzada.");
        }
        else
        {
        }
    }
}
