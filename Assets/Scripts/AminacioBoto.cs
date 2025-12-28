using UnityEngine;

public class AnimacioBoto : MonoBehaviour
{
    public Transform placa;
    public float desplazamientoY = 0.1f;
    public float velocidad = 10f;

    private Vector3 posicionInicial;
    private bool pulsado;

    void Start()
    {
        posicionInicial = placa.localPosition;
    }

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

    public void SetPulsado(bool estado)
    {
        pulsado = estado;
    }
}
