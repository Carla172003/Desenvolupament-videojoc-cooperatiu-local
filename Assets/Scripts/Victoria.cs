using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controlador de l'objecte de victòria.
/// Permet activar l'objecte de victòria quan es compleixen les condicions del nivell.
/// </summary>
public class Victoria : MonoBehaviour
{
    /// <summary>
    /// Activa l'objecte de victòria (normalment un panell UI).
    /// </summary>
    public void ActivarVictoria()
    {
        gameObject.SetActive(true);
    }
}