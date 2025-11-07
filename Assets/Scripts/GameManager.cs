using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private PuntColocacio[] puntsColocacio;
    private bool hasGuanyat = false;

    // Start is called before the first frame update
    void Start()
    {
        puntsColocacio = FindObjectsOfType<PuntColocacio>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ComprovarVictoria()
    {
        if (hasGuanyat) return; // Evita comprobar más si ya ganaste

        bool totsColocats = true;

        // Recorre todos los puntos de colocación
        foreach (PuntColocacio punt in puntsColocacio)
        {
            if (!punt.ocupat)
            {
                totsColocats = false;
                break; // Si uno no está ocupado, salimos
            }
        }

        if (totsColocats)
        {
            hasGuanyat = true;
            Debug.Log("Has guanyat! Tots els objectes estan col·locats!");
            // Aquí podrías activar una animación, pasar de escena, mostrar UI, etc.
        }
    }
}
