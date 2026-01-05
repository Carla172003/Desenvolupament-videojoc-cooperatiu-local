using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

/// <summary>
/// Botó de pressió que activa plataformes quan un o més jugadors hi estan a sobre.
/// Compta quants jugadors estan prement el botó i activa/desactiva la plataforma associada.
/// </summary>
public class BotoPlataforma : MonoBehaviour
{
    public ControladorPlataforma plataforma;
    public AnimacioBoto animacio;


    private int jugadoresEncima = 0;

    /// <summary>
    /// Detecta quan un jugador entra al botó.
    /// Incrementa el comptador de jugadors i activa la plataforma i l'animació.
    /// </summary>
    /// <param name="other">El collider que ha entrat.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Jugador1") || other.CompareTag("Jugador2"))
        {
            jugadoresEncima++;
            plataforma.SetSubir(true);
            animacio.SetPulsado(true);

        }
    }

    /// <summary>
    /// Detecta quan un jugador surt del botó.
    /// Decrementa el comptador i desactiva la plataforma si no hi ha cap jugador.
    /// </summary>
    /// <param name="other">El collider que ha sortit.</param>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Jugador1") || other.CompareTag("Jugador2"))
        {
            jugadoresEncima--;

            if (jugadoresEncima <= 0)
            {
                plataforma.SetSubir(false);
                animacio.SetPulsado(false);
                jugadoresEncima = 0;
            }
        }
    }
}
