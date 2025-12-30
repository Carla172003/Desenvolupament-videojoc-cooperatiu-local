using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicaMenuController : MonoBehaviour
{
    public static MusicaMenuController instancia;
    public static bool mantenerMusica = false;

    // Start is called before the first frame update
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

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Si se carga la escena de juego, destruir este objeto
        if (scene.name == "Nivell1")
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
