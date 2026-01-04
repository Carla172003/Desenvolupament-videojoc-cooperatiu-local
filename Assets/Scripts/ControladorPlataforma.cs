using UnityEngine;
using UnityEngine.Audio;    

/// <summary>
/// Controlador de plataformes mòbils verticals.
/// Mou la plataforma entre una alçada mínima i màxima quan s'activa.
/// Reprodueix so en bucle mentre es mou.
/// </summary>
public class ControladorPlataforma : MonoBehaviour
{
    public float velocidad = 2f;
    public float alturaMaxima = 2.76f;
    public float alturaMinima = -7.28f;

    public AudioClip movimientoClip;

    private bool subir = false;
    private float ultimaY;
    private bool sonando = false;

    /// <summary>
    /// Guarda la posició Y inicial de la plataforma.
    /// </summary>
    void Start()
    {
        ultimaY = transform.position.y;
    }

    /// <summary>
    /// Mou la plataforma cada frame segons l'estat de subir.
    /// Gestiona el so de moviment en funció de si s'està movent.
    /// </summary>
    void Update()
    {
        float nuevaY = transform.position.y;

        if (subir)
            nuevaY += velocidad * Time.deltaTime;
        else
            nuevaY -= velocidad * Time.deltaTime;

        nuevaY = Mathf.Clamp(nuevaY, alturaMinima, alturaMaxima);

        transform.position = new Vector3(
            transform.position.x,
            nuevaY,
            transform.position.z
        );

        bool seEstaMoviendo = Mathf.Abs(nuevaY - ultimaY) > 0.001f;

        if (seEstaMoviendo && !sonando)
        {
            ControladorSo.Instance.ReproduirSoEnBucle(movimientoClip);
            sonando = true;
        }
        else if (!seEstaMoviendo && sonando)
        {
            ControladorSo.Instance.AturarSo();
            sonando = false;
        }

        ultimaY = nuevaY;
    }

    /// <summary>
    /// Estableix si la plataforma ha de pujar o baixar.
    /// </summary>
    /// <param name="estado">True per pujar, false per baixar.</param>
    public void SetSubir(bool estado)
    {
        subir = estado;
    }
}
