using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LeaderboardWheelInput : MonoBehaviour
{
    // =========================================================
    // LETTERS
    // =========================================================

    [Header("Letter Selectors")]
    [SerializeField] private LetterCarousel[] letterCarousels;


    // =========================================================
    // INPUT
    // =========================================================

    [Header("Wheel Inputs")]

    [Tooltip("Palanca izquierda")]
    [SerializeField] private InputActionReference leftButtonAction;

    [Tooltip("Palanca derecha")]
    [SerializeField] private InputActionReference rightButtonAction;

    [Tooltip("Pedal del acelerador")]
    [SerializeField] private InputActionReference acceleratorAction;

    [Tooltip("Pedal del freno")]
    [SerializeField] private InputActionReference brakeAction;


    // =========================================================
    // PEDALS
    // =========================================================

    [Header("Pedal Settings")]

    [Tooltip(
        "Valor por debajo del cual consideramos que el pedal está siendo pisado. " +
        "En vuestro Logitech 1 significa pedal suelto."
    )]
    [SerializeField] private float pedalPressedThreshold = 0.8f;

    [Tooltip(
        "Valor a partir del cual consideramos que el pedal está completamente suelto."
    )]
    [SerializeField] private float pedalReleasedThreshold = 0.95f;


    // =========================================================
    // SCENE
    // =========================================================

    [Header("Scene")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";


    // =========================================================
    // INTERNAL
    // =========================================================

    private int currentSlot = 0;

    private bool scoreSaved = false;

    // Los pedales no se habilitan hasta que primero
    // los hayamos visto completamente sueltos.
    private bool acceleratorReady = false;
    private bool brakeReady = false;

    // Estado del frame anterior.
    private bool acceleratorWasPressed = false;
    private bool brakeWasPressed = false;


    // =========================================================
    // UNITY
    // =========================================================

    private void Start()
    {
        if (letterCarousels == null || letterCarousels.Length == 0)
        {
            Debug.LogError(
                "LeaderboardWheelInput: no hay LetterCarousels asignados."
            );

            enabled = false;
            return;
        }

        currentSlot = 0;

        UpdateSelectedSlot();

        Debug.Log(
            "Tiempo recibido de la carrera: "
            + RaceResultData.FinalTime
        );
    }


    private void OnEnable()
    {
        // PALANCAS
        if (leftButtonAction != null)
        {
            leftButtonAction.action.performed += OnLeftPressed;
            leftButtonAction.action.Enable();
        }

        if (rightButtonAction != null)
        {
            rightButtonAction.action.performed += OnRightPressed;
            rightButtonAction.action.Enable();
        }


        // PEDALES
        // Los habilitamos, pero NO usamos performed.
        if (acceleratorAction != null)
        {
            acceleratorAction.action.Enable();
        }

        if (brakeAction != null)
        {
            brakeAction.action.Enable();
        }
    }


    private void OnDisable()
    {
        if (leftButtonAction != null)
        {
            leftButtonAction.action.performed -= OnLeftPressed;
            leftButtonAction.action.Disable();
        }

        if (rightButtonAction != null)
        {
            rightButtonAction.action.performed -= OnRightPressed;
            rightButtonAction.action.Disable();
        }

        if (acceleratorAction != null)
        {
            acceleratorAction.action.Disable();
        }

        if (brakeAction != null)
        {
            brakeAction.action.Disable();
        }
    }


    private void Update()
    {
        if (scoreSaved)
            return;

        ReadAccelerator();
        ReadBrake();
    }


    // =========================================================
    // PADDLES - CHANGE LETTER
    // =========================================================

    private void OnLeftPressed(
        InputAction.CallbackContext context)
    {
        if (scoreSaved)
            return;
    }

    private void OnRightPressed(
        InputAction.CallbackContext context)
    {
        if (scoreSaved)
            return;

        Debug.Log(
            "LEADERBOARD: palanca derecha"
        );

        letterCarousels[currentSlot]
            .NextLetter();
    }


    // =========================================================
    // ACCELERATOR - NEXT / SAVE
    // =========================================================

    private void ReadAccelerator()
    {
        if (acceleratorAction == null)
            return;


        float rawValue =
            acceleratorAction.action.ReadValue<float>();


        // Primero tenemos que haber detectado
        // el pedal completamente suelto.
        if (!acceleratorReady)
        {
            if (rawValue >= pedalReleasedThreshold)
            {
                acceleratorReady = true;
                acceleratorWasPressed = false;

                Debug.Log(
                    "LEADERBOARD: acelerador preparado"
                );
            }

            return;
        }


        bool acceleratorPressed =
            rawValue < pedalPressedThreshold;


        // Solo actuamos en el momento:
        //
        // SUELTO -> PISADO
        if (
            acceleratorPressed &&
            !acceleratorWasPressed
        )
        {
            AcceleratorPressed();
        }


        acceleratorWasPressed =
            acceleratorPressed;
    }


    private void AcceleratorPressed()
    {
        Debug.Log(
            "LEADERBOARD: acelerador pulsado"
        );


        // Si todavía quedan letras,
        // avanzamos al siguiente slot.
        if (
            currentSlot <
            letterCarousels.Length - 1
        )
        {
            currentSlot++;

            UpdateSelectedSlot();

            return;
        }


        // Si estábamos ya en la tercera letra,
        // guardamos directamente.
        SaveScore();
    }


    // =========================================================
    // BRAKE - BACK
    // =========================================================

    private void ReadBrake()
    {
        if (brakeAction == null)
            return;


        float rawValue =
            brakeAction.action.ReadValue<float>();


        // Igual que el acelerador:
        // esperamos primero a encontrarlo suelto.
        if (!brakeReady)
        {
            if (rawValue >= pedalReleasedThreshold)
            {
                brakeReady = true;
                brakeWasPressed = false;

                Debug.Log(
                    "LEADERBOARD: freno preparado"
                );
            }

            return;
        }


        bool brakePressed =
            rawValue < pedalPressedThreshold;


        // Solo una acción por cada pulsación.
        if (
            brakePressed &&
            !brakeWasPressed
        )
        {
            BrakePressed();
        }


        brakeWasPressed =
            brakePressed;
    }


    private void BrakePressed()
    {
        Debug.Log(
            "LEADERBOARD: freno pulsado"
        );


        if (currentSlot <= 0)
            return;


        currentSlot--;

        UpdateSelectedSlot();
    }


    // =========================================================
    // ACTIVE LETTER FEEDBACK
    // =========================================================

    private void UpdateSelectedSlot()
    {
        for (
            int i = 0;
            i < letterCarousels.Length;
            i++
        )
        {
            letterCarousels[i]
                .SetSelected(
                    i == currentSlot
                );
        }


        Debug.Log(
            "LEADERBOARD: editando letra "
            + (currentSlot + 1)
        );
    }


    // =========================================================
    // NAME
    // =========================================================

    private string GetPlayerName()
    {
        string playerName = "";


        foreach (
            LetterCarousel carousel
            in letterCarousels
        )
        {
            playerName +=
                carousel.GetCurrentLetter();
        }


        return playerName;
    }


    // =========================================================
    // SAVE
    // =========================================================

    private void SaveScore()
    {
        if (scoreSaved)
            return;


        scoreSaved = true;


        string playerName =
            GetPlayerName();


        float finalTime =
            RaceResultData.FinalTime;


        LeaderboardManager.AddScore(
            playerName,
            finalTime
        );


        Debug.Log(
            "LEADERBOARD GUARDADO: "
            + playerName
            + " | "
            + finalTime
        );


        Time.timeScale = 1f;


        SceneManager.LoadScene(
            mainMenuSceneName
        );
    }
}