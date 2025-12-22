using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MostrarPuntuacioNivell : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textPuntuacio;

    void Start()
    {
        int puntuacioFinal = GameManager.Instance.puntuacio;
        textPuntuacio = GetComponent<TextMeshProUGUI>();
        textPuntuacio.text = puntuacioFinal + " pts";
    }
}
