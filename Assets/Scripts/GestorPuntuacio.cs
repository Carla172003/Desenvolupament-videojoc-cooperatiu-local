using UnityEngine;

/// <summary>
/// Gestor de puntuació que gestiona el càlcul de punts, puntuació final i estrelles.
/// Utilitza el patró Strategy per al càlcul d'estrelles.
/// </summary>
public class GestorPuntuacio
{
    private int puntuacio;
    private ISistemaPuntuacio estrategiaEstrelles;

    public GestorPuntuacio(ISistemaPuntuacio estrategia)
    {
        this.estrategiaEstrelles = estrategia;
        this.puntuacio = 0;
    }

    /// <summary>
    /// Obté la puntuació actual.
    /// </summary>
    public int ObtenirPuntuacio()
    {
        return puntuacio;
    }

    /// <summary>
    /// Reinicia la puntuació a 0.
    /// </summary>
    public void ReiniciarPuntuacio()
    {
        puntuacio = 0;
    }

    /// <summary>
    /// Suma punts a la puntuació actual.
    /// </summary>
    /// <param name="punts">Quantitat de punts a sumar.</param>
    public void SumarPunts(int punts)
    {
        puntuacio += punts;
    }

    /// <summary>
    /// Calcula la puntuació final sumant el temps restant a la puntuació actual.
    /// </summary>
    /// <param name="tempsRestant">Temps restant en segons.</param>
    /// <returns>Puntuació final calculada.</returns>
    public int CalcularPuntuacioFinal(float tempsRestant)
    {
        int puntuacioFinal = puntuacio + Mathf.RoundToInt(tempsRestant);
        return puntuacioFinal;
    }

    /// <summary>
    /// Calcula el nombre d'estrelles utilitzant l'estratègia configurada.
    /// </summary>
    /// <param name="puntuacioFinal">Puntuació final de la partida.</param>
    /// <param name="partidaCompletada">True si la partida s'ha completat amb èxit.</param>
    /// <returns>Nombre d'estrelles (0-3).</returns>
    public int CalcularEstrelles(int puntuacioFinal, bool partidaCompletada)
    {
        return estrategiaEstrelles.CalcularEstrelles(puntuacioFinal, partidaCompletada);
    }

    /// <summary>
    /// Canvia l'estratègia de càlcul d'estrelles.
    /// </summary>
    /// <param name="novaEstrategia">Nova estratègia a utilitzar.</param>
    public void CanviarEstrategia(ISistemaPuntuacio novaEstrategia)
    {
        this.estrategiaEstrelles = novaEstrategia;
    }
}
