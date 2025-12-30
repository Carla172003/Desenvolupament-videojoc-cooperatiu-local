using System.Collections.Generic;
using UnityEngine;

public class ZonaObjectes : MonoBehaviour
{
    [Header("Objectes possibles (prefabs)")]
    public List<GameObject> objectesPossibles; 

    [Header("Punt on es instancien")]
    public Transform puntSpawn;

    private KeyCode specialKeyJugador1 = KeyCode.E;
    private KeyCode specialKeyJugador2 = KeyCode.O;

    private bool jugador1Dins = false;
    private bool jugador2Dins = false;

    private bool hayObjetoDentro = false; // true si ya hay un objeto en la zona o el jugador entra con uno

    void Update()
    {
        // Solo generar si no hay objeto dentro
        if (!hayObjetoDentro)
        {
            if (jugador1Dins && Input.GetKeyDown(specialKeyJugador1))
            {
                GenerarObjecteRandom();
            }

            if (jugador2Dins && Input.GetKeyDown(specialKeyJugador2))
            {
                GenerarObjecteRandom();
            }
        }
    }

    private void GenerarObjecteRandom()
    {
        if (objectesPossibles == null || objectesPossibles.Count == 0)
            return;

        int index = Random.Range(0, objectesPossibles.Count);
        GameObject prefab = objectesPossibles[index];

        // Crear nuevo objeto en el punto de spawn
        GameObject nuevoObjeto = Instantiate(prefab, puntSpawn.position, Quaternion.identity);
        nuevoObjeto.name = prefab.name;

        // Marcar que ahora hay objeto dentro
        hayObjetoDentro = true;

        // Eliminar prefab de la lista para no repetir
        objectesPossibles.RemoveAt(index);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Jugador1"))
            jugador1Dins = true;
        else if (other.CompareTag("Jugador2"))
            jugador2Dins = true;

        // Si el jugador entra con un objeto (por ejemplo un objeto que tenga un componente ControladorObjecte)
        if (other.GetComponent<ControladorObjecte>() != null)
            hayObjetoDentro = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Jugador1"))
            jugador1Dins = false;
        else if (other.CompareTag("Jugador2"))
            jugador2Dins = false;

        // Si el objeto sale de la zona, permitimos generar otro
        if (other.GetComponent<ControladorObjecte>() != null)
            hayObjetoDentro = false;
    }
}
