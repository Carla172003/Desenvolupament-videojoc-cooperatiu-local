using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Mostra la puntuació final del nivell a la pantalla de victòria.
/// Obté la puntuació del GameManager i l'assigna al text UI.
/// </summary>
public class MostrarPuntuacioNivell : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textPuntuacio;

    /// <summary>
    /// Obté la puntuació del GameManager i l'assigna al component de text.
    /// </summary>
    void Start()
    {
        int puntuacioFinal = GameManager.Instance.puntuacio;
        textPuntuacio = GetComponent<TextMeshProUGUI>();
        textPuntuacio.text = puntuacioFinal + " pts";
    }
}
