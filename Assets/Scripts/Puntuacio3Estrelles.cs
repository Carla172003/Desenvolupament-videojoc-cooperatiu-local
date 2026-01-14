using UnityEngine;

/// <summary>
/// Estratègia per calcular estrelles basada en 3 llindars de puntuació.
/// </summary>
public class Puntuacio3Estrelles : ISistemaPuntuacio
{
    private int puntuacioMin2Estrelles;
    private int puntuacioMin3Estrelles;

    public Puntuacio3Estrelles(int llindarDuesEstrelles, int llindarTresEstrelles)
    {
        puntuacioMin2Estrelles = llindarDuesEstrelles;
        puntuacioMin3Estrelles = llindarTresEstrelles;
    }

    /// <summary>
    /// Calcula el nombre d'estrelles obtingudes segons la puntuació final.
    /// Si partida no completada: 0 estrelles.
    /// Si completada: compara amb els llindars definits.
    /// </summary>
    /// <param name="puntuacioFinal">Puntuació final de la partida.</param>
    /// <param name="partidaCompletada">True si la partida s'ha completat amb èxit.</param>
    /// <returns>Nombre d'estrelles (0-3).</returns>
    public int CalcularEstrelles(int puntuacioFinal, bool partidaCompletada)
    {
        // 0 estrelles si la partida no s'ha completat
        if (!partidaCompletada)
        {
            return 0;
        }

        // Determinar estrelles segons els llindars configurats
        if (puntuacioFinal >= puntuacioMin3Estrelles)
        {
            return 3;
        }
        else if (puntuacioFinal >= puntuacioMin2Estrelles)
        {
            return 2;
        }
        else
        {
            return 1;
        }
    }
}
