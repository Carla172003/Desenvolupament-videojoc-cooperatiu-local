using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private ControladorEscena controladorEscena;
    
    private PuntColocacio[] puntsColocacio;
    private bool hasGuanyat = false;
    private bool hasPerdut = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        controladorEscena = FindObjectOfType<ControladorEscena>();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Escena cargada: " + scene.name);

        // Solo inicializar la partida cuando entramos a Nivell1
        if (scene.name == "Nivell1")
        {
            IniciarPartida();
        }
    }
    
    public void IniciarPartida()
    {
        hasGuanyat = false;
        hasPerdut = false;

        Debug.Log("Partida iniciada.");
        establirLlistaPunts();
    }

    private void establirLlistaPunts()
    {
        Debug.Log("Establint llista de punts de col·locació.");
        puntsColocacio = FindObjectsOfType<PuntColocacio>();
        if (puntsColocacio == null)
        {
            Debug.LogWarning("No s'han trobat punts de col·locació a l'escena actual.");
        }
        Debug.Log("S'han trobat " + puntsColocacio.Length + " punts de col·locació.");
        foreach (PuntColocacio punt in puntsColocacio)
        {
            Debug.Log("Punt de col·locació establert: " + punt.idCorrecte);
        }
    }

    public void ComprovarVictoria()
    {
        if (hasGuanyat || hasPerdut) return; 

        bool totsColocats = true;

        foreach (PuntColocacio punt in puntsColocacio)
        {
            if (!punt.ocupat)
            {
                totsColocats = false;
                break;
            }
        }

        if (totsColocats)
        {
            hasGuanyat = true;
            controladorEscena.CarregarEscena("Victoria");
        }
    }

    public void PerderPartida()
    {
        if (hasGuanyat || hasPerdut) return; // Evita comprobar si ya terminó

        hasPerdut = true;
        controladorEscena.CarregarEscena("Derrota");
    }

}
