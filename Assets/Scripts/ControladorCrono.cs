using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ControladorCrono : MonoBehaviour
{
    [Header("Tiempo de juego (segundos)")]
    public float tempsMaxim = 120f; // 2 minutos por ejemplo
    private float tempsRestant;
    public bool cronoActiu = false;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI cronoUI; // Cambiado a serializedField

    // Start is called before the first frame update
    void Start()
    {
        EstablirCrono();
    }

    // Update is called once per frame
    void Update()
    {
        if (cronoActiu)
        {
            RestarTemps();
        }
    }

    public void EstablirCrono()
    {
        tempsRestant = tempsMaxim;
        ActivarCrono();
    }

    public void PararCrono()
    {
        cronoActiu = false;
        Time.timeScale = 0f;
    }

    public void ActivarCrono()
    {
        cronoActiu = true;
        Time.timeScale = 1f;
    }

    private void RestarTemps()
    {
        tempsRestant -= Time.deltaTime;
        if (tempsRestant <= 0)
        {
            GameManager.Instance.PerderPartida();
            tempsRestant = 0;
        }
        ActualizarUI();
    }
    
    private void ActualizarUI()
    {
        // Formato MM:SS
        int minutos = Mathf.FloorToInt(tempsRestant / 60);
        int segundos = Mathf.FloorToInt(tempsRestant % 60);

        cronoUI.text = $"{minutos:00}:{segundos:00}";
    }
}
