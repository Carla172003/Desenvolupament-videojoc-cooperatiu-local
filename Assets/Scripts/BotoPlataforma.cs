using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class BotoPlataforma : MonoBehaviour
{
    public ControladorPlataforma plataforma;
    public AnimacioBoto animacio;
    public AudioClip botonClip;


    private int jugadoresEncima = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Jugador1") || other.CompareTag("Jugador2"))
        {
            jugadoresEncima++;
            plataforma.SetSubir(true);
            animacio.SetPulsado(true);

            AudioManager.Instance.ReproduirSoEnBucle(botonClip);

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
                AudioManager.Instance.AturarSo();
            }
        }
    }
}
