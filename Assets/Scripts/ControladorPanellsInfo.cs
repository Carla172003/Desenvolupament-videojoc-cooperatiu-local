using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gestiona els panells d'instruccions inicials de cada nivell.
/// </summary>
public class ControladorPanellsInfo : MonoBehaviour
{
    [Header("Panells d'instruccions")]
    public GameObject[] panells;

    private int panellActual = 0;

    /// <summary>
    /// Mostra les instruccions del nivell.
    /// Activa el primer panell i pausa el joc.
    /// </summary>
    public void MostrarInstruccions()
    {
        panellActual = 0;


        for (int i = 0; i < panells.Length; i++)
        {
            panells[i].SetActive(i == panellActual);
        }

        RectTransform rt = panells[0].GetComponent<RectTransform>();


        // Pausar el joc mientras se muestran las instrucciones
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Avança al següent panell d'instruccions.
    /// Si és l'últim panell, reprendre el joc i inicia la partida.
    /// Cridat pel botó "Següent" de la UI.
    /// </summary>
    public void SeguentPanell()
    {
        // Desactivar panel actual
        panells[panellActual].SetActive(false);

        panellActual++;

        if (panellActual < panells.Length)
        {
            panells[panellActual].SetActive(true);
        }
        else
        {
            // Último panel: reanudar el juego
            Time.timeScale = 1f;

            // Llamar a GameManager para iniciar la partida
            if (GameManager.Instance != null)
            {
                GameManager.Instance.IniciarPartida();
            }
        }
    }
}
