using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

/// <summary>
/// Gestor principal del joc, controla l'estat general de la partida i la transició d'escenes.
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance { get; private set; }
    #endregion

    #region Constants
    private const string TAG_PANEL_FADE = "PanelFade";
    private const string ESCENA_VICTORIA = "Victoria";
    private const string ESCENA_DERROTA = "Derrota";
    private const string PREFIX_NIVELL = "Nivell";
    #endregion

    #region Configuració
    [Header("Configuració de Fade")]
    [SerializeField] private Image panelFade;
    [SerializeField] private float duracioFade = 1f;
    #endregion

    #region Controladors
    private ControladorEscena controladorEscena;
    private ControladorPuntuacio controladorPuntuacio;
    private ControladorCrono controladorCrono;
    private ControladorPanellsInfo controladorPanells;
    #endregion
    
    #region Estat de la partida
    private PuntColocacio[] puntsColocacio;
    private bool finalitzada = false;
    public int puntuacio = 0;
    public int numEstrelles = 0;
    private string nivellActual = ""; // Guarda el nom del nivell actual
    
    // Estratègies de validació
    private List<IValidacioEstrategia> estrategiesValidacio;
    
    // Patró estat per gestionar estat de la partida
    private EstatPartida estatActual;
    #endregion

    #region Inicialització Singleton
    /// <summary>
    /// Assegura que només hi hagi una instància del GameManager i que persisteixi entre escenes.
    /// Implementa el patró Singleton per garantir una única instància global del GameManager.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
    #endregion

    #region Gestió d'Escenes
    /// <summary>
    /// Gestiona la càrrega d'escenes. S'executa automàticament quan es carrega una nova escena.
    /// Inicialitza els controladors necessaris segons el tipus d'escena.
    /// </summary>
    /// <param name="scene">L'escena que s'ha carregat.</param>
    /// <param name="mode">El mode de càrrega de l'escena.</param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InicialitzarControladors();
        ConfigurarPanelFade(scene.name);
        
        if (EsNivell(scene.name))
        {
            InicialitzarNivell();
        }
    }

    /// <summary>
    /// Inicialitza o reassigna els controladors de l'escena actual.
    /// </summary>
    private void InicialitzarControladors()
    {
        controladorEscena = FindObjectOfType<ControladorEscena>();
    }

    /// <summary>
    /// Configura el panel de fade per a l'escena actual.
    /// </summary>
    /// <param name="nomEscena">Nom de l'escena carregada.</param>
    private void ConfigurarPanelFade(string nomEscena)
    {
        GameObject panelObj = GameObject.FindGameObjectWithTag(TAG_PANEL_FADE);
        if (panelObj != null)
        {
            panelFade = panelObj.GetComponent<Image>();
            
            if (EsEscenaFinal(nomEscena))
            {
                StartCoroutine(FadeInEscena());
            }
        }
    }

    /// <summary>
    /// Inicialitza els controladors específics d'un nivell.
    /// </summary>
    private void InicialitzarNivell()
    {
        StartCoroutine(MostrarPanelesCoroutine());
        controladorPuntuacio = FindObjectOfType<ControladorPuntuacio>();
        controladorCrono = FindObjectOfType<ControladorCrono>();
        
        // Netejar dependències d'objectes al iniciar el nivell
        ControladorObjecte.NetejaDependencies();
    }

    /// <summary>
    /// Comprova si l'escena és un nivell jugable.
    /// </summary>
    private bool EsNivell(string nomEscena)
    {
        return nomEscena.StartsWith(PREFIX_NIVELL);
    }

    /// <summary>
    /// Comprova si l'escena és Victoria o Derrota.
    /// </summary>
    private bool EsEscenaFinal(string nomEscena)
    {
        return nomEscena == ESCENA_VICTORIA || nomEscena == ESCENA_DERROTA;
    }
    #endregion

    #region Gestió de Partida
    /// <summary>
    /// Corutina que espera un frame abans de mostrar els panells d'instruccions.
    /// Això garanteix que tots els objectes de l'escena estiguin inicialitzats.
    /// </summary>
    private IEnumerator MostrarPanelesCoroutine()
    {
        yield return new WaitForEndOfFrame();

        controladorPanells = FindObjectOfType<ControladorPanellsInfo>();
        if (controladorPanells != null)
        {
            controladorPanells.MostrarInstruccions();
        }
        else
        {
            IniciarPartida();
        }
    }
    
    /// <summary>
    /// Inicia la partida. Reinicia els valors de finalització i puntuació.
    /// </summary>
    public void IniciarPartida()
    {
        finalitzada = false;
        puntuacio = 0;
        nivellActual = SceneManager.GetActiveScene().name;
        puntsColocacio = FindObjectsOfType<PuntColocacio>();
        
        // Inicialitzar estratègies de validació
        InicialitzarEstrategiesValidacio();
        
        // Inicialitzar estat com a Jugant
        CanviarEstat(new EstatJugant(this));
    }

    /// <summary>
    /// Inicialitza la llista d'estratègies de validació segons el patró Strategy.
    /// Cada estratègia valida un tipus específic d'objectes (Attrezzo, Llums, Vestimenta).
    /// </summary>
    private void InicialitzarEstrategiesValidacio()
    {
        estrategiesValidacio = new List<IValidacioEstrategia>
        {
            new ValidacioAttrezzo(),
            new ValidacioLlums(),
            new ValidacioVestimenta()
        };
    }

    /// <summary>
    /// Comprova si s'ha aconseguit la victòria utilitzant les estratègies de validació.
    /// Aplica el patró Strategy per validar els diferents tipus d'objectes.
    /// </summary>
    public void ComprovarVictoria()
    {
        if (finalitzada) return;
        if (estatActual == null || !estatActual.PotComprovarVictoria()) return;

        // Validar amb totes les estratègies
        foreach (IValidacioEstrategia estrategia in estrategiesValidacio)
        {
            if (!estrategia.Validar())
            {
                return; // Si alguna validació falla, no hi ha victòria
            }
        }

        // Si totes les validacions passen, victòria!
        estatActual.FinalitzarAmbVictoria();
    }

    /// <summary>
    /// Processa la victòria. Cridat per EstatFinalitzada.
    /// </summary>
    public void ProcessarVictoria()
    {
        finalitzada = true;
        controladorCrono.PararCrono();
        puntuacio = controladorPuntuacio.CalcularPuntuacioFinal(controladorCrono.tempsRestant * 100);
        
        // Calcular estrelles obtingudes i guardar-les al GameManager
        numEstrelles = controladorPuntuacio.CalcularEstrelles(puntuacio, true);
        
        // Guardar les dades del nivell si s'ha millorat
        if (GestorDadesNivells.Instance != null)
        {
            GestorDadesNivells.Instance.GuardarDadesNivell(nivellActual, puntuacio, numEstrelles);
        }
        
        AudioClip clipVictoria = ControladorSo.Instance?.clipVictoria;
        StartCoroutine(TransicioEscena(ESCENA_VICTORIA, clipVictoria));
    }

    /// <summary>
    /// Finalitza la partida amb derrota.
    /// </summary>
    public void PerderPartida()
    {
        if (finalitzada) return;
        if (estatActual != null)
        {
            estatActual.FinalitzarAmbDerrota();
        }
    }

    /// <summary>
    /// Processa la derrota. Cridat per EstatFinalitzada.
    /// </summary>
    public void ProcessarDerrota()
    {
        finalitzada = true;
        
        // Guardar la puntuació actual del ControladorPuntuacio al GameManager
        if (controladorPuntuacio != null)
        {
            puntuacio = controladorPuntuacio.puntuacio;
        }
        
        AudioClip clipDerrota = ControladorSo.Instance?.clipDerrota;
        StartCoroutine(TransicioEscena(ESCENA_DERROTA, clipDerrota));
    }

    /// <summary>
    /// Retorna la puntuació del nivell actual.
    /// </summary>
    public int GetPuntuacio()
    {
        return puntuacio;
    }

    /// <summary>
    /// Canvia l'estat actual de la partida.
    /// </summary>
    public void CanviarEstat(EstatPartida nouEstat)
    {
        estatActual?.OnExit();
        estatActual = nouEstat;
        estatActual?.OnEnter();
    }

    /// <summary>
    /// Pausa la partida.
    /// </summary>
    public void Pausar()
    {
        estatActual?.Pausar();
    }

    /// <summary>
    /// Reprèn la partida.
    /// </summary>
    public void Reprendre()
    {
        estatActual?.Reprendre();
    }
    #endregion

    #region Transicions d'Escena
    /// <summary>
    /// Corutina que gestiona la transició entre escenes amb fade out i so.
    /// </summary>
    /// <param name="nomEscena">Nom de l'escena de destí.</param>
    /// <param name="clip">Clip d'àudio a reproduir durant la transició.</param>
    private IEnumerator TransicioEscena(string nomEscena, AudioClip clip = null)
    {
        if (clip != null)
        {
            ReproduirSo(clip);
        }
        
        yield return FadeOutAmbPanel();
        CarregarEscena(nomEscena);
    }

    /// <summary>
    /// Reprodueix un so si ControladorSo està disponible.
    /// </summary>
    private void ReproduirSo(AudioClip clip)
    {
        if (ControladorSo.Instance != null && clip != null)
        {
            ControladorSo.Instance.ReproduirSoUncop(clip);
        }
    }

    /// <summary>
    /// Carrega una escena pel seu nom.
    /// </summary>
    private void CarregarEscena(string nomEscena)
    {
        if (controladorEscena != null)
        {
            controladorEscena.CarregarEscena(nomEscena);
        }
    }
    #endregion

    #region Sistema de Fade
    /// <summary>
    /// Realitza un fade out bloquejant els raycasts.
    /// </summary>
    private IEnumerator FadeOutAmbPanel()
    {
        if (panelFade != null)
        {
            panelFade.raycastTarget = true;
            yield return Fade(0f, 1f);
        }
    }

    /// <summary>
    /// Corutina que fa un fade d'opacitat del panel.
    /// </summary>
    /// <param name="alphaInicial">Valor alfa inicial (0 = transparent, 1 = opac).</param>
    /// <param name="alphaFinal">Valor alfa final (0 = transparent, 1 = opac).</param>
    private IEnumerator Fade(float alphaInicial, float alphaFinal)
    {
        float tempsTranscorregut = 0f;
        Color color = panelFade.color;

        while (tempsTranscorregut < duracioFade)
        {
            tempsTranscorregut += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(alphaInicial, alphaFinal, tempsTranscorregut / duracioFade);
            color.a = alpha;
            panelFade.color = color;
            yield return null;
        }

        color.a = alphaFinal;
        panelFade.color = color;
    }

    /// <summary>
    /// Corutina que fa un fade in automàtic per a escenes finals.
    /// </summary>
    private IEnumerator FadeInEscena()
    {
        if (panelFade == null) yield break;

        // Assegurar que comença en negre
        Color color = panelFade.color;
        color.a = 1f;
        panelFade.color = color;
        
        yield return Fade(1f, 0f);
        
        // Desactivar raycast perquè no bloquegi clicks
        panelFade.raycastTarget = false;
    }
    #endregion
}
