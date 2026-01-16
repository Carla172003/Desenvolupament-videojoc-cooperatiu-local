using UnityEngine;

/// <summary>
/// Interfície per a les estratègies de càlcul d'estrelles.
/// Implementa el patró Strategy per permetre diferents sistemes de càlcul d'estrelles.
/// </summary>
public interface ISistemaPuntuacio
{
    /// <summary>
    /// Calcula el nombre d'estrelles obtingudes segons la puntuació final.
    /// </summary>
    /// <param name="puntuacioFinal">Puntuació final de la partida.</param>
    /// <param name="partidaCompletada">True si la partida s'ha completat amb èxit.</param>
    /// <returns>Nombre d'estrelles (0-3).</returns>
    int CalcularEstrelles(int puntuacioFinal, bool partidaCompletada);
}
