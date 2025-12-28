using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using System.Collections;
using UnityEngine.Localization;
using TMPro;

public class LanguageDropdown : MonoBehaviour
{
    public TMP_Dropdown languageDropdown;

    void Start()
    {
        if (languageDropdown == null) Debug.LogError("LanguageDropdown: languageDropdown is not assigned.");
        languageDropdown = GetComponent<TMP_Dropdown>();

        // Opcional: establecer el valor inicial según la lengua actual
        languageDropdown.value = GetCurrentLanguageIndex();
        languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
    }

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

    public void OnLanguageChanged(int index)
    {
        // Esto cambia el idioma global
        StartCoroutine(ChangeLocale(index));
    }

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
