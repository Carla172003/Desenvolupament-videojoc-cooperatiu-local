using UnityEngine;

/// <summary>
/// Classe base abstracta per als estats del jugador.
/// Defineix el comportament comú que han d'implementar tots els estats.
/// </summary>
public abstract class EstatJugador
{
    protected MovimentsJugadors jugador;

    public EstatJugador(MovimentsJugadors jugador)
    {
        this.jugador = jugador;
    }

    /// <summary>
    /// Processa l'input del jugador en l'estat actual.
    /// </summary>
    public abstract void ProcessarInput();

    /// <summary>
    /// Actualitza la física del jugador en l'estat actual.
    /// </summary>
    public abstract void ActualitzarFisica();

    /// <summary>
    /// Comprova si l'estat ha de canviar a un altre.
    /// </summary>
    public abstract EstatJugador ComprovarTransicions();
}
