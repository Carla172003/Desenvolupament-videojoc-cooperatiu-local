using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using System.Collections;
using UnityEngine.Localization;
using TMPro;

/// <summary>
/// Controlador del desplegable de selecció d'idioma.
/// Permet als usuaris canviar entre català (ca), espanyol (es) i anglès (en).
/// Utilitza el sistema de localització de Unity.
/// </summary>
public class LanguageDropdown : MonoBehaviour
{
    public TMP_Dropdown languageDropdown;

    /// <summary>
    /// Inicialitza el desplegable amb l'idioma actual seleccionat.
    /// </summary>
    void Start()
    {
        if (languageDropdown == null) return;
        languageDropdown = GetComponent<TMP_Dropdown>();

        // Opcional: establecer el valor inicial según la lengua actual
        languageDropdown.value = GetCurrentLanguageIndex();
        languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
    }

    /// <summary>
    /// Obté l'índex del desplegable segons l'idioma actual.
    /// </summary>
    /// <returns>0 per català, 1 per espanyol, 2 per anglès.</returns>
    int GetCurrentLanguageIndex()
    {
        var currentLocale = LocalizationSettings.SelectedLocale.Identifier.Code;
        switch (currentLocale)
        {
            case "ca": return 0;
            case "es": return 1;
            case "en": return 2;
            default: return 1; // por defecto español
        }
    }

    /// <summary>
    /// S'executa quan l'usuari canvia l'idioma al desplegable.
    /// Inicia la corutina per canviar la locale.
    /// </summary>
    /// <param name="index">Índex de l'idioma seleccionat.</param>
    public void OnLanguageChanged(int index)
    {
        // Esto cambia el idioma global
        StartCoroutine(ChangeLocale(index));
    }

    /// <summary>
    /// Corutina que canvia l'idioma del joc.
    /// </summary>
    /// <param name="index">Índex de l'idioma a establir.</param>
    /// <returns>IEnumerator per a la corutina.</returns>
    IEnumerator ChangeLocale(int index)
    {
        var locales = LocalizationSettings.AvailableLocales.Locales;

        Locale newLocale = index switch
        {
            0 => locales.Find(l => l.Identifier.Code == "ca"),
            1 => locales.Find(l => l.Identifier.Code == "es"),
            2 => locales.Find(l => l.Identifier.Code == "en"),
            _ => locales[1]
        };

        LocalizationSettings.SelectedLocale = newLocale;

        // Esperar un frame para que se actualicen los textos
        yield return null;
    }
}
