using UnityEngine;

/// <summary>
/// Interfície per als observadors de canvis en la puntuació.
/// Implementa el patró Observer per notificar canvis de puntuació.
/// </summary>
public interface IObservadorPuntuacio
{
    /// <summary>
    /// Mètode cridat quan es modifica la puntuació.
    /// </summary>
    /// <param name="novaPuntuacio">La nova puntuació.</param>
    void ActualitzarPuntuacio(int novaPuntuacio);
}
