using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controlador d'un botó de nivell a la pantalla de selecció de nivells.
/// Mostra les estrelles obtingudes per aquest nivell i gestiona el bloqueig.
/// </summary>
public class BotoNivell : MonoBehaviour
{
    [Header("Configuració del nivell")]
    [SerializeField] private string nomNivell; // Ex: "Nivell1", "Nivell2", "Nivell3"
    [SerializeField] private string nomNivellAnterior; // Ex: "" per Nivell1, "Nivell1" per Nivell2, etc.
    
    [Header("Components d'imatges de les estrelles")]
    [SerializeField] private Image estrella1;
    [SerializeField] private Image estrella2;
    [SerializeField] private Image estrella3;
    
    [Header("Sprites d'estrelles")]
    [SerializeField] private Sprite estrellaPlena;
    [SerializeField] private Sprite estrellaVacia;
    
    [Header("Opcional: Text de puntuació")]
    [SerializeField] private TextMeshProUGUI textPuntuacio;
    
    [Header("Bloqueig del nivell")]
    [SerializeField] private GameObject iconaCadenat; // Icona de cadenat quan està bloquejat
    
    private Button boto;
    private bool estaBloquejarNivell = false;

    /// <summary>
    /// Inicialitza el botó carregant les dades guardades del nivell.
    /// </summary>
    void Start()
    {
        boto = GetComponent<Button>();
        ActualitzarEstrelles();
        ComprovarBloqueig();
    }

    /// <summary>
    /// Actualitza la visualització de les estrelles basant-se en les dades guardades.
    /// </summary>
    public void ActualitzarEstrelles()
    {
        if (GestorDadesNivells.Instance == null)
        {
            return;
        }

        int estrellesObtingudes = GestorDadesNivells.Instance.ObtenirEstrellesMaximes(nomNivell);
        int puntuacioMaxima = GestorDadesNivells.Instance.ObtenirPuntuacioMaxima(nomNivell);

        // Actualitzar sprites de les estrelles
        if (estrella1 != null && estrellaPlena != null && estrellaVacia != null)
        {
            estrella1.sprite = estrellesObtingudes >= 1 ? estrellaPlena : estrellaVacia;
            estrella2.sprite = estrellesObtingudes >= 2 ? estrellaPlena : estrellaVacia;
            estrella3.sprite = estrellesObtingudes >= 3 ? estrellaPlena : estrellaVacia;
            
            // Restaurar alpha a 1 per defecte
            SetAlpha(estrella1, 1f);
            SetAlpha(estrella2, 1f);
            SetAlpha(estrella3, 1f);
        }

        // Opcional: mostrar puntuació màxima
        if (textPuntuacio != null && puntuacioMaxima > 0)
        {
            textPuntuacio.text = puntuacioMaxima + " pts";
        }
        else if (textPuntuacio != null)
        {
            textPuntuacio.text = "";
        }
        
        // Actualitzar estat de bloqueig després d'actualitzar estrelles
        ComprovarBloqueig();
    }

    /// <summary>
    /// Comprova si el nivell està bloquejat basant-se en el progrés del nivell anterior.
    /// </summary>
    private void ComprovarBloqueig()
    {
        // El primer nivell sempre està desbloquejat
        if (string.IsNullOrEmpty(nomNivellAnterior))
        {
            estaBloquejarNivell = false;
            if (iconaCadenat != null) iconaCadenat.SetActive(false);
            if (boto != null) boto.interactable = true;
            return;
        }

        // Comprovar si el nivell anterior té almenys 1 estrella
        if (GestorDadesNivells.Instance != null)
        {
            int estrellesAnterior = GestorDadesNivells.Instance.ObtenirEstrellesMaximes(nomNivellAnterior);
            estaBloquejarNivell = estrellesAnterior < 1;
        }
        else
        {
            estaBloquejarNivell = true; // Si no hi ha gestor, bloquejar per seguretat
        }

        // Actualitzar UI segons estat de bloqueig
        if (iconaCadenat != null) iconaCadenat.SetActive(estaBloquejarNivell);
        if (boto != null) boto.interactable = !estaBloquejarNivell;
        
        // Opcional: atenuar les estrelles si està bloquejat
        if (estaBloquejarNivell)
        {
            float alpha = 0.3f;
            if (estrella1 != null) SetAlpha(estrella1, alpha);
            if (estrella2 != null) SetAlpha(estrella2, alpha);
            if (estrella3 != null) SetAlpha(estrella3, alpha);
        }
    }

    /// <summary>
    /// Ajusta l'alpha d'una imatge.
    /// </summary>
    private void SetAlpha(Image img, float alpha)
    {
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }
}
