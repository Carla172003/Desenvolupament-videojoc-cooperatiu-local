using UnityEngine;

/// <summary>
/// Animació visual del botó de plataforma.
/// Mou la placa del botó cap avall quan està pulsat i la torna a pujar quan no ho està.
/// </summary>
public class AnimacioBoto : MonoBehaviour
{
    public Transform placa;
    public float desplazamientoY = 0.1f;
    public float velocidad = 10f;

    private Vector3 posicionInicial;
    private bool pulsado;

    /// <summary>
    /// Guarda la posició inicial de la placa del botó.
    /// </summary>
    void Start()
    {
        posicionInicial = placa.localPosition;
    }

    /// <summary>
    /// Anima la placa del botó suavitzadament cap a la posició objectiu.
    /// Si està pulsat, baixa. Si no, torna a la posició inicial.
    /// </summary>
    void Update()
    {
        Vector3 destino = pulsado
            ? posicionInicial + Vector3.down * desplazamientoY
            : posicionInicial;

        placa.localPosition = Vector3.Lerp(
            placa.localPosition,
            destino,
            Time.deltaTime * velocidad
        );
    }

    /// <summary>
    /// Estableix l'estat de pulsat del botó.
    /// </summary>
    /// <param name="estado">True si el botó està pulsat, false si no.</param>
    public void SetPulsado(bool estado)
    {
        pulsado = estado;
    }
}
