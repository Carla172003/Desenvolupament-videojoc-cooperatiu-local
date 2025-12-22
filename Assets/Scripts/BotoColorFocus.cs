using UnityEngine;

public class BotonColorFocus : MonoBehaviour
{
    private GameObject jugadorCerca = null;

    [Header("Punt de colocació asociado")]
    public PuntColocacio puntAsociado;

    [Header("Teclas por jugador")]
    public KeyCode teclaJugador1 = KeyCode.E;
    public KeyCode teclaJugador2 = KeyCode.O;

    void Update()
    {
        if (jugadorCerca == null) return;

        // Detecta el jugador y usa la tecla correspondiente
        if ((jugadorCerca.CompareTag("Jugador1") && Input.GetKeyDown(teclaJugador1)) ||
            (jugadorCerca.CompareTag("Jugador2") && Input.GetKeyDown(teclaJugador2)))
        {
            if (puntAsociado != null)
                puntAsociado.CambiarColorAleatorio();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Jugador1") || other.CompareTag("Jugador2"))
            jugadorCerca = other.gameObject;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (jugadorCerca == other.gameObject)
            jugadorCerca = null;
    }
}
