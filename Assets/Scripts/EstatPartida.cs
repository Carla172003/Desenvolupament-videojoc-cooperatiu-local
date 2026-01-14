using UnityEngine;

/// <summary>
/// Classe base abstracta per als diferents estats de la partida.
/// Implementa el patró State per gestionar els estats: Jugant, Pausada i Finalitzada.
/// </summary>
public abstract class EstatPartida
{
    protected GameManager gameManager;

    public EstatPartida(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }

    /// <summary>
    /// Mètode cridat quan s'entra en aquest estat.
    /// </summary>
    public virtual void OnEnter() { }

    /// <summary>
    /// Mètode cridat quan se surt d'aquest estat.
    /// </summary>
    public virtual void OnExit() { }

    /// <summary>
    /// Intenta pausar la partida. Només és vàlid des de l'estat Jugant.
    /// </summary>
    public virtual void Pausar() { }

    /// <summary>
    /// Intenta reprendre la partida. Només és vàlid des de l'estat Pausada.
    /// </summary>
    public virtual void Reprendre() { }

    /// <summary>
    /// Intenta finalitzar la partida amb victòria.
    /// </summary>
    public virtual void FinalitzarAmbVictoria() { }

    /// <summary>
    /// Intenta finalitzar la partida amb derrota.
    /// </summary>
    public virtual void FinalitzarAmbDerrota() { }

    /// <summary>
    /// Comprova si es pot validar la victòria en aquest estat.
    /// </summary>
    /// <returns>True si es pot comprovar la victòria, false si no.</returns>
    public virtual bool PotComprovarVictoria()
    {
        return false;
    }
}
