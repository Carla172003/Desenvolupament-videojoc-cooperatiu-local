using UnityEngine;

/// <summary>
/// Estat Saltant: gestiona el moviment del jugador mentre està a l'aire.
/// Manté el control horitzontal durant el salt i torna a EstatCaminar quan aterra.
/// </summary>
public class EstatSaltant : EstatJugador
{
    private bool haSaltat = false;
    private bool aplicarForcaSalt = false;

    public EstatSaltant(MovimentsJugadors jugador, bool aplicarSalt = true) : base(jugador) 
    {
        // Marcar que entrem saltant
        haSaltat = false;
        aplicarForcaSalt = aplicarSalt;
    }

    public override void ProcessarInput()
    {
        // Moviment horitzontal (control a l'aire)
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

        // Aplicar salt només si s'ha premut la tecla (no quan cau naturalment)
        bool aplicarSalt = !haSaltat && aplicarForcaSalt;
        if (!haSaltat)
        {
            haSaltat = true;
        }

        // Aplicar moviment horitzontal i salt
        jugador.ExecutarMoviment(jugador.movimentHorizontal * Time.fixedDeltaTime, aplicarSalt);
    }

    public override EstatJugador ComprovarTransicions()
    {
        // Transició a estat Escales si està en contacte amb una escala
        // i prem amunt o avall, i NO està agafant un objecte
        if (jugador.escales && 
            (Input.GetKeyDown(jugador.upKey) || Input.GetKeyDown(jugador.downKey)) &&
            !(jugador.agafarObjecte != null && jugador.agafarObjecte.teObjecte))
        {
            return new EstatPujantBaixantEscales(jugador);
        }

        // Tornar a estat Caminant quan aterra
        if (jugador.estaAterra && haSaltat)
        {
            return new EstatCaminant(jugador);
        }

        return null; // Continuar en aquest estat
    }
}
