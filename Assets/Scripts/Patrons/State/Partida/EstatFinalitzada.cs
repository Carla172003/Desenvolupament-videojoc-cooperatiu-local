using UnityEngine;

/// <summary>
/// Estat quan la partida ha finalitzat (victòria o derrota).
/// No es pot reprendre ni pausar des d'aquest estat.
/// </summary>
public class EstatFinalitzada : EstatPartida
{
    private bool esVictoria;

    public EstatFinalitzada(GameManager gameManager, bool esVictoria) : base(gameManager)
    {
        this.esVictoria = esVictoria;
    }

    public override void OnEnter()
    {
        if (esVictoria)
        {
            gameManager.ProcessarVictoria();
        }
        else
        {
            gameManager.ProcessarDerrota();
        }
    }

    public override bool PotComprovarVictoria()
    {
        return false; // La partida ja ha finalitzat
    }
}
