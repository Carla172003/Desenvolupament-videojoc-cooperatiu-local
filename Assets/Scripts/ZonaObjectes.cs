using System.Collections.Generic;
using UnityEngine;

public class ZonaObjectes : MonoBehaviour
{
    [Header("Objetos posibles (prefabs)")]
    public List<GameObject> objectesPossibles;  // ⚠️ Cambiado a List para poder quitar elementos

    [Header("Punto donde se instancian")]
    public Transform puntSpawn;

    private bool jugadorProper = false;
    private KeyCode specialKey = KeyCode.E;

    private void Start()
    {
        AssignarTecles();
    }

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
            Debug.Log("🎯 Ja s'han generat tots els objectes possibles!");
            return;
        }

        int index = Random.Range(0, objectesPossibles.Count);
        GameObject prefab = objectesPossibles[index];

        // Crear el objeto
        GameObject nouObjecte = Instantiate(prefab, puntSpawn.position, Quaternion.identity);
        nouObjecte.name = prefab.name;

        Debug.Log($"🧱 Objecte generat: {nouObjecte.name}");

        // Asignar su punto correcto
        ObjecteColocable objScript = nouObjecte.GetComponent<ObjecteColocable>();
        if (objScript != null)
        {
            PuntColocacio[] punts = FindObjectsOfType<PuntColocacio>();
            foreach (var punt in punts)
            {
                if (punt.idCorrecte == objScript.idObjecte)
                {
                    objScript.puntCorrecte = punt;
                    Debug.Log($"🧩 Assignat punt correcte: {punt.name} a {objScript.idObjecte}");
                    break;
                }
            }
        }

        // 👇 Eliminar el prefab usado de la lista
        objectesPossibles.RemoveAt(index);
        Debug.Log($"📦 {prefab.name} eliminat de la llista. Queden {objectesPossibles.Count} objectes per generar.");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Jugador1") || other.CompareTag("Jugador2"))
        {
            jugadorProper = true;
            Debug.Log("Jugador a prop del magatzem (pulsa E per generar objecte)");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Jugador1") || other.CompareTag("Jugador2"))
        {
            jugadorProper = false;
        }
    }

    private void AssignarTecles()
    {
        if (CompareTag("Jugador1"))
            specialKey = KeyCode.E;
        else if (CompareTag("Jugador2"))
            specialKey = KeyCode.O;
    }
}
