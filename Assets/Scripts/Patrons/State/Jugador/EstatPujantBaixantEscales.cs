using UnityEngine;

/// <summary>
/// Estat Pujant/Baixant Escales: gestiona el moviment vertical quan el jugador està enganxat a una escala.
/// El jugador no pot moure's horitzontalment mentre està en aquest estat,
/// però pot desenganxar-se prement les tecles esquerra o dreta.
/// </summary>
public class EstatPujantBaixantEscales : EstatJugador
{
    private float movimentVertical = 0f;

    public EstatPujantBaixantEscales(MovimentsJugadors jugador) : base(jugador) 
    {
        // En entrar a l'estat, enganxar a l'escala
        EngantxarAEscala();
    }

    private void EngantxarAEscala()
    {
        jugador.rb.gravityScale = 0f;
        jugador.rb.velocity = Vector2.zero;

        // Centrar al jugador en l'escala
        if (jugador.centreEscales != null)
        {
            Vector3 pos = jugador.transform.position;
            pos.x = jugador.centreEscales.position.x;
            jugador.transform.position = pos;
        }
    }

    public override void ProcessarInput()
    {
        // No permet moviment horitzontal mentre està a l'escala
        jugador.movimentHorizontal = 0f;

        // Moviment vertical
        if (Input.GetKey(jugador.upKey))
            movimentVertical = jugador.velocitatEscales;
        else if (Input.GetKey(jugador.downKey))
            movimentVertical = -jugador.velocitatEscales;
        else
            movimentVertical = 0f;

        // Animacions
        jugador.animator.SetFloat("Horizontal", 0f);
        jugador.animator.SetFloat("VelocitatY", movimentVertical);
    }

    public override void ActualitzarFisica()
    {
        // Actualitzar estats animacions
        jugador.animator.SetBool("estaAterra", false);
        jugador.animator.SetBool("estaEscales", true);

        // Aplicar moviment vertical
        jugador.rb.velocity = new Vector2(0f, movimentVertical);
    }

    public override EstatJugador ComprovarTransicions()
    {
        // Desenganxar si prem esquerra o dreta
        if (Input.GetKeyDown(jugador.leftKey) || Input.GetKeyDown(jugador.rightKey))
        {
            jugador.rb.gravityScale = jugador.gravetat;
            return new EstatCaminant(jugador);
        }

        // Sortir de l'escala si ja no està en contacte
        if (!jugador.escales)
        {
            jugador.rb.gravityScale = jugador.gravetat;
            return new EstatCaminant(jugador);
        }

        return null; // Continuar en aquest estat
    }
}
