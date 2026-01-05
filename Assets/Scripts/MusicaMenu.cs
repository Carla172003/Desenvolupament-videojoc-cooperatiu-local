using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controlador de la música del menú principal.
/// Manoté la música entre escenes de menú però la destrueix quan es carrega un nivell.
/// </summary>
public class MusicaMenuController : MonoBehaviour
{
    public static MusicaMenuController instancia;
    public static bool mantenerMusica = false;

    /// <summary>
    /// Implementa el patró Singleton per assegurar una única instància de música de menú.
    /// </summary>
    void Awake()
    {
        if (instancia != null && instancia != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instancia = this;
        }
    }

    /// <summary>
    /// Registra el listener d'escenes quan s'activa.
    /// </summary>
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>
    /// Desregistra el listener d'escenes quan es desactiva.
    /// </summary>
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// S'executa quan es carrega una nova escena.
    /// Si és un nivell, destrueix la música del menú. Si no, la manté.
    /// </summary>
    /// <param name="scene">L'escena carregada.</param>
    /// <param name="mode">El mode de càrrega.</param>
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Si se carga la escena de juego, destruir este objeto
        if (scene.name.StartsWith("Nivell"))
        {
            Destroy(gameObject);
            instancia = null;
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
