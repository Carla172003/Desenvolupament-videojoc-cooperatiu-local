using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gestor de dades dels nivells.
/// Guarda les puntuacions i estrelles només durant la sessió actual (no es persisten entre execucions).
/// </summary>
public class GestorDadesNivells : MonoBehaviour
{
    public static GestorDadesNivells Instance;

    // Diccionaris per guardar dades en memòria durant la sessió
    private Dictionary<string, int> puntuacionsNivells = new Dictionary<string, int>();
    private Dictionary<string, int> estrellesNivells = new Dictionary<string, int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Aplicar DontDestroyOnLoad al root GameObject
            DontDestroyOnLoad(transform.root.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Guarda la puntuació i les estrelles d'un nivell si és una millora (només en memòria).
    /// </summary>
    /// <param name="nomNivell">Nom del nivell (ex: "Nivell1").</param>
    /// <param name="puntuacio">Puntuació obtinguda.</param>
    /// <param name="estrelles">Nombre d'estrelles obtingudes (0-3).</param>
    public void GuardarDadesNivell(string nomNivell, int puntuacio, int estrelles)
    {
        int puntuacioMaxima = ObtenirPuntuacioMaxima(nomNivell);
        
        // Només guardar si és una millora
        if (puntuacio > puntuacioMaxima)
        {
            puntuacionsNivells[nomNivell] = puntuacio;
            estrellesNivells[nomNivell] = estrelles;
        }
    }

    /// <summary>
    /// Obté la puntuació màxima d'un nivell durant aquesta sessió.
    /// </summary>
    /// <param name="nomNivell">Nom del nivell (ex: "Nivell1").</param>
    /// <returns>Puntuació màxima obtinguda, o 0 si no s'ha jugat.</returns>
    public int ObtenirPuntuacioMaxima(string nomNivell)
    {
        if (puntuacionsNivells.ContainsKey(nomNivell))
        {
            return puntuacionsNivells[nomNivell];
        }
        return 0;
    }

    /// <summary>
    /// Obté el nombre d'estrelles màximes obtingudes en un nivell durant aquesta sessió.
    /// </summary>
    /// <param name="nomNivell">Nom del nivell (ex: "Nivell1").</param>
    /// <returns>Nombre d'estrelles (0-3).</returns>
    public int ObtenirEstrellesMaximes(string nomNivell)
    {
        if (estrellesNivells.ContainsKey(nomNivell))
        {
            return estrellesNivells[nomNivell];
        }
        return 0;
    }

    /// <summary>
    /// Esborra totes les dades guardades de la sessió actual.
    /// </summary>
    public void EsborrarTotsElsNivells()
    {
        puntuacionsNivells.Clear();
        estrellesNivells.Clear();
    }
}
