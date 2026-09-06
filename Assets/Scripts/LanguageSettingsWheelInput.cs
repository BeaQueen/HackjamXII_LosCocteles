using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class LanguageSettingsWheelInput : MonoBehaviour
{
    // =========================================================
    // UI
    // =========================================================

    [Header("UI")]

    [Tooltip("Dropdown que selecciona el idioma")]
    [SerializeField] private TMP_Dropdown languageDropdown;

    [Tooltip("Botón Back para volver al menú principal")]
    [SerializeField] private Button backButton;


    // =========================================================
    // INPUT
    // =========================================================

    [Header("Wheel Inputs")]

    [Tooltip("Palanca izquierda")]
    [SerializeField] private InputActionReference leftPaddleAction;

    [Tooltip("Palanca derecha")]
    [SerializeField] private InputActionReference rightPaddleAction;

    [Tooltip("Pedal del freno")]
    [SerializeField] private InputActionReference brakeAction;


    // =========================================================
    // PEDAL SETTINGS
    // =========================================================

    [Header("Pedal Settings")]

    [Tooltip("Valor por debajo del cual consideramos el pedal pisado")]
    [SerializeField] private float pedalPressedThreshold = 0.8f;

    [Tooltip("Valor a partir del cual consideramos el pedal suelto")]
    [SerializeField] private float pedalReleasedThreshold = 0.95f;


    // =========================================================
    // INTERNAL
    // =========================================================

    private bool brakeReady = false;
    private bool brakeWasPressed = false;


    // =========================================================
    // UNITY
    // =========================================================

    private void Start()
    {
        if (languageDropdown == null)
        {
            Debug.LogError(
                "LanguageSettingsWheelInput: No hay Language Dropdown asignado."
            );
        }
    }


    private void OnEnable()
    {
        // PALANCA IZQUIERDA
        if (leftPaddleAction != null)
        {
            leftPaddleAction.action.performed += OnLeftPaddlePressed;
            leftPaddleAction.action.Enable();
        }


        // PALANCA DERECHA
        if (rightPaddleAction != null)
        {
            rightPaddleAction.action.performed += OnRightPaddlePressed;
            rightPaddleAction.action.Enable();
        }


        // FRENO
        if (brakeAction != null)
        {
            brakeAction.action.Enable();
        }
    }


    private void OnDisable()
    {
        if (leftPaddleAction != null)
        {
            leftPaddleAction.action.performed -= OnLeftPaddlePressed;
            leftPaddleAction.action.Disable();
        }


        if (rightPaddleAction != null)
        {
            rightPaddleAction.action.performed -= OnRightPaddlePressed;
            rightPaddleAction.action.Disable();
        }


        if (brakeAction != null)
        {
            brakeAction.action.Disable();
        }
    }


    private void Update()
    {
        ReadBrake();


        // =====================================================
        // DEBUG CON TECLADO
        // =====================================================

        if (Keyboard.current != null)
        {
            // Flecha izquierda = palanca izquierda
            if (
                Keyboard.current
                    .leftArrowKey
                    .wasPressedThisFrame
            )
            {
                PreviousLanguage();
            }


            // Flecha derecha = palanca derecha
            if (
                Keyboard.current
                    .rightArrowKey
                    .wasPressedThisFrame
            )
            {
                NextLanguage();
            }


            // Escape / Backspace = freno
            if (
                Keyboard.current
                    .escapeKey
                    .wasPressedThisFrame
                ||
                Keyboard.current
                    .backspaceKey
                    .wasPressedThisFrame
            )
            {
                GoBack();
            }
        }
    }


    // =========================================================
    // LEFT PADDLE
    // =========================================================

    private void OnLeftPaddlePressed(
        InputAction.CallbackContext context)
    {
        Debug.Log(
            "SETTINGS: PALANCA IZQUIERDA"
        );

        PreviousLanguage();
    }


    // =========================================================
    // RIGHT PADDLE
    // =========================================================

    private void OnRightPaddlePressed(
        InputAction.CallbackContext context)
    {
        Debug.Log(
            "SETTINGS: PALANCA DERECHA"
        );

        NextLanguage();
    }


    // =========================================================
    // PREVIOUS LANGUAGE
    // =========================================================

    private void PreviousLanguage()
    {
        if (languageDropdown == null)
            return;

        if (languageDropdown.options.Count == 0)
            return;


        int newValue =
            languageDropdown.value - 1;


        // Si estamos en la primera opción,
        // saltamos a la última.
        if (newValue < 0)
        {
            newValue =
                languageDropdown.options.Count - 1;
        }


        languageDropdown.value =
            newValue;


        Debug.Log(
            "SETTINGS: idioma = "
            + languageDropdown
                .options[newValue]
                .text
        );
    }


    // =========================================================
    // NEXT LANGUAGE
    // =========================================================

    private void NextLanguage()
    {
        if (languageDropdown == null)
            return;

        if (languageDropdown.options.Count == 0)
            return;


        int newValue =
            languageDropdown.value + 1;


        // Si estamos en la última opción,
        // volvemos a la primera.
        if (
            newValue >=
            languageDropdown.options.Count
        )
        {
            newValue = 0;
        }


        languageDropdown.value =
            newValue;


        Debug.Log(
            "SETTINGS: idioma = "
            + languageDropdown
                .options[newValue]
                .text
        );
    }


    // =========================================================
    // BRAKE
    // =========================================================

    private void ReadBrake()
    {
        if (brakeAction == null)
            return;


        float rawValue =
            brakeAction.action
                .ReadValue<float>();


        // Primero esperamos a que Unity
        // detecte el pedal completamente suelto.
        if (!brakeReady)
        {
            if (
                rawValue >=
                pedalReleasedThreshold
            )
            {
                brakeReady = true;
                brakeWasPressed = false;
            }

            return;
        }


        bool pressed =
            rawValue <
            pedalPressedThreshold;


        // Solo una acción al empezar
        // a pisar el pedal.
        if (
            pressed &&
            !brakeWasPressed
        )
        {
            GoBack();
        }


        brakeWasPressed =
            pressed;
    }


    // =========================================================
    // BACK
    // =========================================================

    private void GoBack()
    {
        if (backButton == null)
            return;


        if (backButton.interactable)
        {
            Debug.Log(
                "SETTINGS: BACK"
            );

            backButton.onClick.Invoke();
        }
    }
}