using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Controlador del cronòmetre del joc.
/// </summary>
public class ControladorCrono : MonoBehaviour
{
    [Header("Temps de joc (segons)")]
    public float tempsMaxim = 120f; // 2 minuts 
    public float tempsRestant;
    public bool cronoActiu = false;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI cronoUI; 

    // Inicia el cronòmetre
    void Start()
    {
        EstablirCrono();
    }

    // Actualitza el cronòmetre cada frame
    void Update()
    {
        if (cronoActiu)
        {
            RestarTemps();
        }
    }

    /// <summary>
    /// Inicialitza el cronòmetre al temps màxim i l'activa.
    /// </summary>
    public void EstablirCrono()
    {
        tempsRestant = tempsMaxim;
        ActivarCrono();
    }

    /// <summary>
    /// Para el cronòmetre i pausa el joc establint Time.timeScale a 0.
    /// </summary>
    public void PararCrono()
    {
        cronoActiu = false;
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Activa el cronòmetre i reprendre el joc establint Time.timeScale a 1.
    /// </summary>
    public void ActivarCrono()
    {
        cronoActiu = true;
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Resta el temps del cronòmetre cada frame.
    /// Quan arriba a zero, crida GameManager per perdre la partida.
    /// </summary>
    private void RestarTemps()
    {
        tempsRestant -= Time.deltaTime;
        if (tempsRestant <= 0)
        {
            GameManager.Instance.PerderPartida();
            tempsRestant = 0;
        }
        ActualitzarUI();
    }
    
    /// <summary>
    /// Actualitza la UI del cronòmetre amb format MM:SS.
    /// </summary>
    private void ActualitzarUI()
    {
        // Format MM:SS
        int minuts = Mathf.FloorToInt(tempsRestant / 60);
        int segons = Mathf.FloorToInt(tempsRestant % 60);

        cronoUI.text = $"{minuts:00}:{segons:00}";
    }
}
