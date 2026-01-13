using UnityEngine;

/// <summary>
/// Botó que permet als jugadors canviar el color d'un spotlight associat.
/// Quan un jugador prem la tecla d'interacció prop del botó,
/// canvia aleatòriament el color del PuntColocacio associat.
/// </summary>
public class BotonColorFocus : MonoBehaviour
{
    private GameObject jugadorCerca = null;

    [Header("Punt de colocació asociado")]
    public PuntColocacio puntAsociado;

    [Header("Teclas por jugador")]
    public KeyCode teclaJugador1 = KeyCode.E;
    public KeyCode teclaJugador2 = KeyCode.O;

    /// <summary>
    /// Comprova cada frame si un jugador proper prem la tecla d'interacció.
    /// Si és així, canvia el color del punt associat.
    /// </summary>
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

    /// <summary>
    /// Detecta quan un jugador entra a l'àrea del botó.
    /// </summary>
    /// <param name="other">El collider que ha entrat.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Jugador1") || other.CompareTag("Jugador2"))
            jugadorCerca = other.gameObject;
    }

    /// <summary>
    /// Detecta quan un jugador surt de l'àrea del botó.
    /// </summary>
    /// <param name="other">El collider que ha sortit.</param>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (jugadorCerca == other.gameObject)
            jugadorCerca = null;
    }
}
