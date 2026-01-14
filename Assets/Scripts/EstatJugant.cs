using UnityEngine;

/// <summary>
/// Estat quan la partida està en curs i els jugadors poden interactuar.
/// </summary>
public class EstatJugant : EstatPartida
{
    public EstatJugant(GameManager gameManager) : base(gameManager) { }

    public override void OnEnter()
    {
        Time.timeScale = 1f; // Assegura que el temps estigui en marxa
    }

    public override void Pausar()
    {
        gameManager.CanviarEstat(new EstatPausada(gameManager));
    }

    public override void FinalitzarAmbVictoria()
    {
        gameManager.CanviarEstat(new EstatFinalitzada(gameManager, true));
    }

    public override void FinalitzarAmbDerrota()
    {
        gameManager.CanviarEstat(new EstatFinalitzada(gameManager, false));
    }

    public override bool PotComprovarVictoria()
    {
        return true; // Només es pot comprovar la victòria quan s'està jugant
    }
}
