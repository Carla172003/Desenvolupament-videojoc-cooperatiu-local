using UnityEngine;

/// <summary>
/// Estat Caminant: gestiona el moviment horitzontal del jugador quan està a terra.
/// Pot transitar a EstatSaltant si el jugador prem la tecla de salt.
/// </summary>
public class EstatCaminant : EstatJugador
{
    public EstatCaminant(MovimentsJugadors jugador) : base(jugador) { }

    public override void ProcessarInput()
    {
        // Moviment horitzontal
        if (Input.GetKey(jugador.leftKey))
            jugador.movimentHorizontal = -jugador.velocitatMoviment;
        else if (Input.GetKey(jugador.rightKey))
            jugador.movimentHorizontal = jugador.velocitatMoviment;
        else
            jugador.movimentHorizontal = 0f;

        // Animacions
        jugador.animator.SetFloat("Horizontal", Mathf.Abs(jugador.movimentHorizontal));
        jugador.animator.SetFloat("VelocitatY", jugador.rb.velocity.y);
    }

    public override void ActualitzarFisica()
    {
        // Comprovar si està a terra
        jugador.ComprovarEstaAterra();

        // Actualitzar estats animacions
        jugador.animator.SetBool("estaAterra", jugador.estaAterra);
        jugador.animator.SetBool("estaEscales", false);

        // Aplicar moviment horitzontal (sense salt)
        jugador.ExecutarMoviment(jugador.movimentHorizontal * Time.fixedDeltaTime, false);
    }

    public override EstatJugador ComprovarTransicions()
    {
        // Transició a estat Escales si el jugador està en contacte amb una escala
        // i prem amunt o avall, i NO està agafant un objecte
        if (jugador.escales && 
            (Input.GetKeyDown(jugador.upKey) || Input.GetKeyDown(jugador.downKey)) &&
            !(jugador.agafarObjecte != null && jugador.agafarObjecte.teObjecte))
        {
            return new EstatPujantBaixantEscales(jugador);
        }

        // Transició a estat Saltant si prem salt i està a terra
        if (Input.GetKeyDown(jugador.upKey) && jugador.estaAterra)
        {
            return new EstatSaltant(jugador, aplicarSalt: true);
        }

        // Si ja no està a terra (per exemple, cau d'un bordell), passar a estat Saltant
        // SENSE aplicar força de salt (només està caient)
        if (!jugador.estaAterra)
        {
            return new EstatSaltant(jugador, aplicarSalt: false);
        }

        return null; // Continuar en aquest estat
    }
}
