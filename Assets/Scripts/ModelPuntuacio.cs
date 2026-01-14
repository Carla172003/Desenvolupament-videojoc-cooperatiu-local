using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Model de domini que gestiona la puntuació del joc.
/// Implementa el patró Observer per notificar canvis als observadors.
/// </summary>
public class ModelPuntuacio
{
    private int puntuacio;
    private List<IObservadorPuntuacio> observadors;

    public ModelPuntuacio()
    {
        puntuacio = 0;
        observadors = new List<IObservadorPuntuacio>();
    }

    /// <summary>
    /// Obté la puntuació actual.
    /// </summary>
    public int ObtenirPuntuacio()
    {
        return puntuacio;
    }

    /// <summary>
    /// Estableix la puntuació i notifica als observadors.
    /// </summary>
    /// <param name="novaPuntuacio">La nova puntuació.</param>
    public void EstablirPuntuacio(int novaPuntuacio)
    {
        puntuacio = novaPuntuacio;
        NotificarObservadors();
    }

    /// <summary>
    /// Suma punts a la puntuació actual i notifica als observadors.
    /// </summary>
    /// <param name="punts">Quantitat de punts a sumar.</param>
    public void SumarPunts(int punts)
    {
        puntuacio += punts;
        NotificarObservadors();
    }

    /// <summary>
    /// Reinicia la puntuació a 0 i notifica als observadors.
    /// </summary>
    public void ReiniciarPuntuacio()
    {
        puntuacio = 0;
        NotificarObservadors();
    }

    /// <summary>
    /// Calcula la puntuació final sumant el temps restant.
    /// </summary>
    /// <param name="tempsRestant">Temps restant en segons.</param>
    /// <returns>Puntuació final calculada.</returns>
    public int CalcularPuntuacioFinal(float tempsRestant)
    {
        return puntuacio + Mathf.RoundToInt(tempsRestant);
    }

    /// <summary>
    /// Subscriu un observador per rebre notificacions de canvis.
    /// </summary>
    /// <param name="observador">L'observador a subscriure.</param>
    public void SubscriureObservador(IObservadorPuntuacio observador)
    {
        if (!observadors.Contains(observador))
        {
            observadors.Add(observador);
        }
    }

    /// <summary>
    /// Desubscriu un observador per deixar de rebre notificacions.
    /// </summary>
    /// <param name="observador">L'observador a desubscriure.</param>
    public void DesubscriureObservador(IObservadorPuntuacio observador)
    {
        observadors.Remove(observador);
    }

    /// <summary>
    /// Notifica a tots els observadors que la puntuació ha canviat.
    /// </summary>
    private void NotificarObservadors()
    {
        foreach (IObservadorPuntuacio observador in observadors)
        {
            observador.ActualitzarPuntuacio(puntuacio);
        }
    }
}
