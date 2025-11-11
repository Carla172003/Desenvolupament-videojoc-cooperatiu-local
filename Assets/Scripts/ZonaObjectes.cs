using System.Collections.Generic;
using UnityEngine;

public class ZonaObjectes : MonoBehaviour
{
    [Header("Objectes possibles (prefabs)")]
    public List<GameObject> objectesPossibles; 

    [Header("Punt on es instancien")]
    public Transform puntSpawn;

    private bool jugadorProper = false;
    private KeyCode specialKey = KeyCode.S;


    void Update()
    {
        if (jugadorProper && Input.GetKeyDown(specialKey))
        {
            GenerarObjecteRandom();
        }
    }

    private void GenerarObjecteRandom()
    {
        if (objectesPossibles == null || objectesPossibles.Count == 0)
        {
            return;
        }

        int index = Random.Range(0, objectesPossibles.Count);
        GameObject prefab = objectesPossibles[index];

        // Crear nou objecte al punt d'spawn
        GameObject nouObjecte = Instantiate(prefab, puntSpawn.position, Quaternion.identity);
        nouObjecte.name = prefab.name;


        // Assignar el punt correcte a l'objecte col·locable
        ControladorObjecte objScript = nouObjecte.GetComponent<ControladorObjecte>();
        if (objScript != null)
        {
            PuntColocacio[] punts = FindObjectsOfType<PuntColocacio>();
            foreach (var punt in punts)
            {
                if (punt.idCorrecte == objScript.idObjecte)
                {
                    objScript.puntCorrecte = punt;
                    break;
                }
            }
        }

        // Eliminar prefab de la llista per no repetir
        objectesPossibles.RemoveAt(index);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Jugador1") || other.CompareTag("Jugador2"))
        {
            jugadorProper = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Jugador1") || other.CompareTag("Jugador2"))
        {
            jugadorProper = false;
        }
    }
}
