using UnityEngine;

/// <summary>
/// Interfície per a les estratègies de validació de la partida.
/// Defineix el mètode que han d'implementar totes les estratègies de validació.
/// </summary>
public interface IValidacioEstrategia
{
    /// <summary>
    /// Valida si els objectes del seu tipus estan col·locats correctament.
    /// </summary>
    /// <returns>True si la validació és correcta, false en cas contrari.</returns>
    bool Validar();
}
