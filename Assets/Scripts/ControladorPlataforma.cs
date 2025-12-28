using UnityEngine;

public class ControladorPlataforma : MonoBehaviour
{
    public float velocidad = 2f;
    public float alturaMaxima = 2.76f;
    public float alturaMinima = -7.28f;

    private bool subir = false;

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
    }

    public void SetSubir(bool estado)
    {
        subir = estado;
    }
}
