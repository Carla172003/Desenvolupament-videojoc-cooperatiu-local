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
    
    [Header("Avís temps crític")]
    [SerializeField] private float tempsAvís = 10f; // Temps en segons per activar l'avís
    [SerializeField] private Color colorCrític = Color.red; // Color quan queden pocs segons
    private Color colorNormal = Color.white; // Color normal del cronòmetre
    private bool avisCritic = false; // Indica si ja s'ha activat l'avís
    
    [Header("So tic tac")]
    [SerializeField] private AudioClip soTicTac; // So que es reprodueix en bucle quan queden pocs segons 

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
        avisCritic = false; // Reiniciar l'avís
        if (cronoUI != null)
        {
            cronoUI.color = colorNormal; // Restaurar color normal
        }
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
    /// Activa avís visual i sonor quan queden pocs segons.
    /// </summary>
    private void RestarTemps()
    {
        tempsRestant -= Time.deltaTime;
        
        // Activar avís crític si queden menys de X segons
        if (tempsRestant <= tempsAvís && !avisCritic)
        {
            avisCritic = true;
            
            // Canviar color del text a vermell
            if (cronoUI != null)
            {
                cronoUI.color = colorCrític;
            }
            
            // Reproduir so de tic tac una vegada (dura 10 segons)
            if (soTicTac != null && ControladorSo.Instance != null)
            {
                ControladorSo.Instance.ReproduirSoUncop(soTicTac);
            }
        }
        
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
