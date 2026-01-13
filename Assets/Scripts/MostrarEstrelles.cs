using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Mostra les estrelles obtingudes a la pantalla de victòria.
/// Canvia els sprites de les imatges segons la puntuació aconseguida.
/// </summary>
public class MostrarEstrelles : MonoBehaviour
{
    [Header("Components d'imatges de les estrelles")]
    [SerializeField] private Image estrella1;
    [SerializeField] private Image estrella2;
    [SerializeField] private Image estrella3;
    
    [Header("Sprites d'estrelles")]
    [SerializeField] private Sprite estrellaPlena;
    [SerializeField] private Sprite estrellaVacia;

    /// <summary>
    /// Obté el nombre d'estrelles del GameManager i les mostra a la UI.
    /// </summary>
    void Start()
    {
        int numEstrelles = GameManager.Instance?.numEstrelles ?? 0;
        MostrarEstrellesObtingudes(numEstrelles);
    }

    /// <summary>
    /// Mostra les estrelles obtingudes canviant els sprites de les imatges.
    /// </summary>
    /// <param name="numEstrelles">Nombre d'estrelles obtingudes (0-3).</param>
    private void MostrarEstrellesObtingudes(int numEstrelles)
    {
        if (estrella1 != null)
        {
            estrella1.sprite = numEstrelles >= 1 ? estrellaPlena : estrellaVacia;
        }
        
        if (estrella2 != null)
        {
            estrella2.sprite = numEstrelles >= 2 ? estrellaPlena : estrellaVacia;
        }
        
        if (estrella3 != null)
        {
            estrella3.sprite = numEstrelles >= 3 ? estrellaPlena : estrellaVacia;
        }
    }
}
