using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private PuntColocacio[] puntsColocacio;
    private bool hasGuanyat = false;
    private bool hasPerdut = false;

    public Victoria victoriaUI;

    [Header("Tiempo de juego (segundos)")]
    public float tempsMaxim = 120f; // 2 minutos por ejemplo
    private float tempsRestant;

    private void Awake()
    {
        // Si no existe una instancia, esta será la principal
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Mantener entre escenas
        }
        else
        {
            // Si ya existe otra instancia, destruir esta
            Destroy(gameObject);
        }
    }

    void Start()
    {
        puntsColocacio = FindObjectsOfType<PuntColocacio>();
        tempsRestant = tempsMaxim;
    }

    void Update()
    {
        if (hasGuanyat || hasPerdut) return;

        // Reducir tiempo
        tempsRestant -= Time.deltaTime;

        if (tempsRestant <= 0)
        {
            tempsRestant = 0;
            hasPerdut = true;
            Debug.Log("Has perdut! S'ha acabat el temps!");
        }

    }

    public void ComprovarVictoria()
    {
        if (hasGuanyat || hasPerdut) return; // Evita comprobar si ya terminó

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
            victoriaUI.ActivarVictoria();
        }
    }
}
