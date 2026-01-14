using UnityEngine;

/// <summary>
/// Estat quan la partida està pausada.
/// El temps s'atura i els jugadors no poden interactuar.
/// </summary>
public class EstatPausada : EstatPartida
{
    public EstatPausada(GameManager gameManager) : base(gameManager) { }

    public override void OnEnter()
    {
        Time.timeScale = 0f; // Pausa el temps del joc
    }

    public override void OnExit()
    {
        Time.timeScale = 1f; // Reanuda el temps del joc
    }

    public override void Reprendre()
    {
        gameManager.CanviarEstat(new EstatJugant(gameManager));
    }

    public override bool PotComprovarVictoria()
    {
        return false; // No es pot comprovar victòria mentre està pausat
    }
}
