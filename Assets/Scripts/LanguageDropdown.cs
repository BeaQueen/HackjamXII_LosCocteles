using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LanguageDropdown : MonoBehaviour
{
    // Referencia al Dropdown de TextMeshPro.
    [SerializeField]
    private TMP_Dropdown languageDropdown;

    // Aquí guardaremos todos los idiomas disponibles que encuentre el sistema de Localization.
    private List<Locale> availableLocales = new List<Locale>();

    private IEnumerator Start()
    {
        // Esperamos a que el sistema de Localization  haya terminado de inicializarse.
        yield return LocalizationSettings.InitializationOperation;

        // Guardamos todos los Locales que tengamos creados en Localization Settings.
        availableLocales.AddRange(
            LocalizationSettings.AvailableLocales.Locales
        );

        // Borramos las opciones que trae el Dropdown por defecto.
        languageDropdown.ClearOptions();

        // Lista con los nombres que aparecerán dentro del Dropdown.
        List<string> languageNames = new List<string>();

        // Recorremos todos los idiomas disponibles.
        foreach (Locale locale in availableLocales)
        {
            // Añadimos el nombre de cada Locale
            languageNames.Add(locale.LocaleName);
        }

        // Añadimos los idiomas encontrados al Dropdown.
        languageDropdown.AddOptions(languageNames);

        // Buscamos qué idioma está actualmente seleccionado.
        int currentLocaleIndex =
            availableLocales.IndexOf(LocalizationSettings.SelectedLocale);

        // Si encontramos el idioma actual...
        if (currentLocaleIndex >= 0)
        {
            // Hacemos que el Dropdown muestre ese idioma sin disparar el evento de cambio.
            languageDropdown.SetValueWithoutNotify(currentLocaleIndex);
        }

        // Actualizamos visualmente el texto mostrado.
        languageDropdown.RefreshShownValue();

        // Cada vez que el usuario cambie una opción llamaremos a ChangeLanguage().
        languageDropdown.onValueChanged.AddListener(ChangeLanguage);
    }

    // Cambia el idioma del juego.

    private void ChangeLanguage(int index)
    {
        // Comprobamos que el índice sea válido.
        if (index < 0 || index >= availableLocales.Count)
        {
            return;
        }

        // Cambiamos el Locale activo.
        LocalizationSettings.SelectedLocale = availableLocales[index];
    }

    private void OnDestroy()
    {
        // Quitamos el listener cuando se destruye el objeto  para evitar referencias innecesarias.
        languageDropdown.onValueChanged.RemoveListener(ChangeLanguage);
    }
}