using UnityEngine;

public class BotoPlataforma : MonoBehaviour
{
    public ControladorPlataforma plataforma;
    public AnimacioBoto animacio;

    private int jugadoresEncima = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Jugador1") || other.CompareTag("Jugador2"))
        {
            jugadoresEncima++;
            plataforma.SetSubir(true);
            animacio.SetPulsado(true);
        }
    }

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
