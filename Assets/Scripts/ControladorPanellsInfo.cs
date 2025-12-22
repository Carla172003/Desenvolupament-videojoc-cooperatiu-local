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

    public void MostrarInstruccions()
    {
        panellActual = 0;

        Debug.Log("Mostrant panell d'instruccions: " + panellActual);

        for (int i = 0; i < panells.Length; i++)
        {
            panells[i].SetActive(i == panellActual);
            Debug.Log($"Panel {i} activo? {panells[i].activeInHierarchy} Pos: {panells[i].transform.position}");
        }

        RectTransform rt = panells[0].GetComponent<RectTransform>();
        Debug.Log($"Panel tamaño: {rt.rect.width}x{rt.rect.height}, pos: {rt.anchoredPosition}");


        // Pausar el joc mientras se muestran las instrucciones
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Método llamado desde el botón "Siguiente"
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
