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

    // Inicialitza el cronòmetre al temps màxim
    public void EstablirCrono()
    {
        tempsRestant = tempsMaxim;
        ActivarCrono();
    }

    // Para el cronòmetre
    public void PararCrono()
    {
        cronoActiu = false;
        Time.timeScale = 0f;
    }

    // Activa el cronòmetre
    public void ActivarCrono()
    {
        cronoActiu = true;
        Time.timeScale = 1f;
    }

    // Resta el temps del cronòmetre cada frame
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
    
    // Actualitza la UI del cronòmetre
    private void ActualitzarUI()
    {
        // Format MM:SS
        int minuts = Mathf.FloorToInt(tempsRestant / 60);
        int segons = Mathf.FloorToInt(tempsRestant % 60);

        cronoUI.text = $"{minuts:00}:{segons:00}";
    }
}
