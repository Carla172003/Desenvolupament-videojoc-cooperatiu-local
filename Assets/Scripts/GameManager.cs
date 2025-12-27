using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Gestor principal del joc, controla l'estat general de la partida i la transició d'escenes.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private ControladorEscena controladorEscena;
    private ControladorPuntuacio controladorPuntuacio;
    private ControladorCrono controladorCrono;
    private ControladorPanellsInfo controladorPanells;
    
    private PuntColocacio[] puntsColocacio;
    private bool finalitzada = false;
    public int puntuacio = 0;

    // Assegura que només hi hagi una instància del GameManager i que persisteixi entre escenes
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // Registra el mètode OnSceneLoaded per gestionar la càrrega d'escenes
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Estableix el controlador d'escena
    void Start()
    {
        controladorEscena = FindObjectOfType<ControladorEscena>();
        
    }

    // Gestiona la càrrega d'escenes
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.StartsWith("Nivell"))
        {
            StartCoroutine(MostrarPanelesCoroutine());
            controladorPuntuacio = FindObjectOfType<ControladorPuntuacio>();
            controladorCrono = FindObjectOfType<ControladorCrono>();
        }
    }

    private IEnumerator MostrarPanelesCoroutine()
    {
        // Esperar un frame para que todos los objetos de la escena estén inicializados
        yield return new WaitForEndOfFrame();

        controladorPanells = FindObjectOfType<ControladorPanellsInfo>();
        if (controladorPanells != null)
        {
            controladorPanells.MostrarInstruccions();
        }
        else
        {
            IniciarPartida();
        }
    }
    
    // Inicia la partida
    public void IniciarPartida()
    {
        finalitzada = false;
        puntuacio = 0;
        agafarLlistaPunts();
    }

    // Agafa la llista dels punts de col·locació d'objectes del nivell
    private void agafarLlistaPunts()
    {
        puntsColocacio = FindObjectsOfType<PuntColocacio>();
    }

    // Comprova si s'ha aconseguit la victòria
    public void ComprovarVictoria()
    {
        if (finalitzada) return; 

        bool totsColocats = true;

        foreach (PuntColocacio punt in puntsColocacio)
        {
            if (!punt.ocupat)
            {
                totsColocats = false;
                break;
            }
            if (!punt.ColorEsCorrecto())
            {
                totsColocats = false;
                break;
            }
        }
        puntuacio = controladorPuntuacio.puntuacio;

        if (totsColocats)
        {
            finalitzada = true;
            controladorCrono.PararCrono();
            puntuacio = controladorPuntuacio.CalcularPuntuacioFinal(controladorCrono.tempsRestant * 100);
            controladorEscena.CarregarEscena("Victoria");
        }
    }

    // Finalitza la partida amb derrotass
    public void PerderPartida()
    {
        if (finalitzada) return; 

        finalitzada = true;
        controladorEscena.CarregarEscena("Derrota");
    }

    // Retorna la puntuació del nivell actual
    public int GetPuntuacio()
    {
        return puntuacio;
    }

}
