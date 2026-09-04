using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LanguageDropdown : MonoBehaviour
{
    // Referencia al Dropdown de TextMeshPro.
    // Arrastraremos el propio Dropdown aquí desde el Inspector.
    [SerializeField]
    private TMP_Dropdown languageDropdown;

    private IEnumerator Start()
    {
        // Esperamos a que Unity Localization haya terminado
        // de cargar todos los idiomas disponibles.
        yield return LocalizationSettings.InitializationOperation;

        // Creamos una lista para guardar las opciones
        // que aparecerán en el Dropdown.
        List<TMP_Dropdown.OptionData> options =
            new List<TMP_Dropdown.OptionData>();

        // Guardaremos aquí el índice del idioma
        // que está seleccionado actualmente.
        int selectedIndex = 0;

        // Recorremos todos los Locales disponibles
        // configurados en Localization Settings.
        for (int i = 0;
             i < LocalizationSettings.AvailableLocales.Locales.Count;
             i++)
        {
            Locale locale =
                LocalizationSettings.AvailableLocales.Locales[i];

            // Añadimos el nombre del idioma al Dropdown.
            options.Add(
                new TMP_Dropdown.OptionData(locale.LocaleName)
            );

            // Si este Locale es el que está actualmente activo,
            // guardamos su posición.
            if (LocalizationSettings.SelectedLocale == locale)
            {
                selectedIndex = i;
            }
        }

        // Sustituimos las opciones que trae el Dropdown
        // por defecto por nuestros idiomas.
        languageDropdown.options = options;

        // Mostramos en el Dropdown el idioma
        // que está actualmente seleccionado.
        languageDropdown.SetValueWithoutNotify(selectedIndex);

        // Actualizamos visualmente el Dropdown.
        languageDropdown.RefreshShownValue();

        // Cuando el usuario seleccione otro idioma,
        // llamamos a ChangeLanguage().
        languageDropdown.onValueChanged.AddListener(ChangeLanguage);

        // Mensaje de prueba.
        Debug.Log(
            "Idioma inicial: " +
            LocalizationSettings.SelectedLocale.LocaleName
        );
    }

    // Esta función recibe el índice seleccionado
    // dentro del Dropdown.
    private void ChangeLanguage(int index)
    {
        // Obtenemos el Locale correspondiente
        // directamente de Localization Settings.
        Locale selectedLocale =
            LocalizationSettings.AvailableLocales.Locales[index];

        // Cambiamos el idioma global del juego.
        LocalizationSettings.SelectedLocale = selectedLocale;

        // Esto nos permitirá comprobar en Console
        // que realmente se ha cambiado.
        Debug.Log(
            "Locale cambiado a: " +
            LocalizationSettings.SelectedLocale.LocaleName
        );
    }
}