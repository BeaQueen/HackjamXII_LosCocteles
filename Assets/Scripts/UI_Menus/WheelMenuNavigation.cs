using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class WheelMenuNavigation : MonoBehaviour
{
    // =========================================================
    // MENU
    // =========================================================

    [Header("Menu")]

    [SerializeField]
    private Button[] menuButtons;

    [SerializeField]
    private Button backButton;

    [SerializeField]
    private RectTransform menuWheel;


    // =========================================================
    // INPUT ACTIONS
    // =========================================================

    [Header("Wheel Inputs")]

    [Tooltip("Palanca izquierda")]
    [SerializeField]
    private InputActionReference leftPaddleAction;

    [Tooltip("Palanca derecha")]
    [SerializeField]
    private InputActionReference rightPaddleAction;

    [Tooltip("Pedal del acelerador")]
    [SerializeField]
    private InputActionReference acceleratorAction;

    [Tooltip("Pedal del freno")]
    [SerializeField]
    private InputActionReference brakeAction;


    // =========================================================
    // ROTATION
    // =========================================================

    [Header("Wheel Rotation")]

    [SerializeField]
    private float rotationSpeed = 500f;

    [Tooltip("Con cuatro botones normalmente será 90")]
    [SerializeField]
    private float anglePerButton = 90f;


    // =========================================================
    // PEDALS
    // =========================================================

    [Header("Pedal Settings")]

    [Tooltip(
        "Valor por debajo del cual consideramos el pedal pisado. " +
        "En vuestro volante el pedal suelto suele dar 1."
    )]
    [SerializeField]
    private float pedalPressedThreshold = 0.8f;

    [Tooltip(
        "Valor a partir del cual consideramos el pedal completamente suelto."
    )]
    [SerializeField]
    private float pedalReleasedThreshold = 0.95f;


    // =========================================================
    // INTERNAL
    // =========================================================

    private int currentButtonIndex = 0;

    private Quaternion targetRotation;

    private bool acceleratorReady = false;
    private bool brakeReady = false;

    private bool acceleratorWasPressed = false;
    private bool brakeWasPressed = false;


    // =========================================================
    // UNITY
    // =========================================================

    private void Start()
    {
        if (menuButtons == null || menuButtons.Length == 0)
        {
            Debug.LogError(
                "WheelMenuNavigation: No hay botones asignados."
            );

            return;
        }


        if (menuWheel == null)
        {
            Debug.LogError(
                "WheelMenuNavigation: No hay Menu Wheel asignada."
            );

            return;
        }


        currentButtonIndex = 0;

        SelectButton(
            currentButtonIndex
        );

        UpdateTargetRotation();
    }


    private void OnEnable()
    {
        // =====================================================
        // PALANCA IZQUIERDA
        // =====================================================

        if (leftPaddleAction != null)
        {
            leftPaddleAction.action.performed +=
                OnLeftPaddlePressed;

            leftPaddleAction.action.Enable();
        }


        // =====================================================
        // PALANCA DERECHA
        // =====================================================

        if (rightPaddleAction != null)
        {
            rightPaddleAction.action.performed +=
                OnRightPaddlePressed;

            rightPaddleAction.action.Enable();
        }


        // =====================================================
        // ACELERADOR
        // =====================================================

        if (acceleratorAction != null)
        {
            acceleratorAction.action.Enable();
        }


        // =====================================================
        // FRENO
        // =====================================================

        if (brakeAction != null)
        {
            brakeAction.action.Enable();
        }
    }


    private void OnDisable()
    {
        if (leftPaddleAction != null)
        {
            leftPaddleAction.action.performed -=
                OnLeftPaddlePressed;

            leftPaddleAction.action.Disable();
        }


        if (rightPaddleAction != null)
        {
            rightPaddleAction.action.performed -=
                OnRightPaddlePressed;

            rightPaddleAction.action.Disable();
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
        // =====================================================
        // TECLADO PARA PROBAR EN CASA
        // =====================================================

        if (Keyboard.current != null)
        {
            // Exactamente lo mismo que palanca izquierda
            if (
                Keyboard.current
                    .leftArrowKey
                    .wasPressedThisFrame
            )
            {
                MoveLeft();
            }


            // Exactamente lo mismo que palanca derecha
            if (
                Keyboard.current
                    .rightArrowKey
                    .wasPressedThisFrame
            )
            {
                MoveRight();
            }


            // Enter = acelerador
            if (
                Keyboard.current
                    .enterKey
                    .wasPressedThisFrame
            )
            {
                SelectCurrentButton();
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


        // =====================================================
        // PEDALES
        // =====================================================

        ReadAccelerator();

        ReadBrake();


        // =====================================================
        // ROTACIÓN VISUAL
        // =====================================================

        if (menuWheel != null)
        {
            menuWheel.localRotation =
                Quaternion.RotateTowards(
                    menuWheel.localRotation,
                    targetRotation,
                    rotationSpeed *
                    Time.unscaledDeltaTime
                );
        }
    }


    // =========================================================
    // LEFT PADDLE
    // =========================================================

    private void OnLeftPaddlePressed(
        InputAction.CallbackContext context)
    {
        Debug.Log(
            "MENU: PALANCA IZQUIERDA"
        );

        // EXACTAMENTE lo mismo que ←
        MoveLeft();
    }


    // =========================================================
    // RIGHT PADDLE
    // =========================================================

    private void OnRightPaddlePressed(
        InputAction.CallbackContext context)
    {
        Debug.Log(
            "MENU: PALANCA DERECHA"
        );

        // EXACTAMENTE lo mismo que →
        MoveRight();
    }


    // =========================================================
    // ACCELERATOR
    // =========================================================

    private void ReadAccelerator()
    {
        if (acceleratorAction == null)
            return;


        float rawValue =
            acceleratorAction.action
                .ReadValue<float>();


        /*
         * Primero esperamos a detectar
         * el pedal suelto.
         *
         * Esto evita que al entrar al menú
         * Unity interprete un valor inicial
         * incorrecto como una pulsación.
         */

        if (!acceleratorReady)
        {
            if (
                rawValue >=
                pedalReleasedThreshold
            )
            {
                acceleratorReady = true;

                acceleratorWasPressed = false;

                Debug.Log(
                    "MENU: acelerador preparado"
                );
            }

            return;
        }


        bool pressed =
            rawValue <
            pedalPressedThreshold;


        // Solo actuamos al pasar de:
        //
        // SUELTO -> PISADO

        if (
            pressed &&
            !acceleratorWasPressed
        )
        {
            Debug.Log(
                "MENU: ACELERADOR / ACCEPT"
            );

            SelectCurrentButton();
        }


        acceleratorWasPressed =
            pressed;
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


        // Primero detectamos el pedal suelto

        if (!brakeReady)
        {
            if (
                rawValue >=
                pedalReleasedThreshold
            )
            {
                brakeReady = true;

                brakeWasPressed = false;

                Debug.Log(
                    "MENU: freno preparado"
                );
            }

            return;
        }


        bool pressed =
            rawValue <
            pedalPressedThreshold;


        // Solo una acción por pulsación

        if (
            pressed &&
            !brakeWasPressed
        )
        {
            Debug.Log(
                "MENU: FRENO / BACK"
            );

            GoBack();
        }


        brakeWasPressed =
            pressed;
    }


    // =========================================================
    // LEFT
    // =========================================================

    private void MoveLeft()
    {
        if (
            menuButtons == null ||
            menuButtons.Length == 0
        )
            return;


        /*
         * Mantiene el comportamiento
         * que tenía tu flecha izquierda.
         */

        MoveSelection(1);
    }


    // =========================================================
    // RIGHT
    // =========================================================

    private void MoveRight()
    {
        if (
            menuButtons == null ||
            menuButtons.Length == 0
        )
            return;


        /*
         * Mantiene el comportamiento
         * que tenía tu flecha derecha.
         */

        MoveSelection(-1);
    }


    // =========================================================
    // CHANGE SELECTION
    // =========================================================

    private void MoveSelection(
        int direction)
    {
        currentButtonIndex += direction;


        // Circular hacia delante
        if (
            currentButtonIndex >=
            menuButtons.Length
        )
        {
            currentButtonIndex = 0;
        }


        // Circular hacia atrás
        else if (
            currentButtonIndex < 0
        )
        {
            currentButtonIndex =
                menuButtons.Length - 1;
        }


        SelectButton(
            currentButtonIndex
        );


        UpdateTargetRotation();
    }


    // =========================================================
    // SELECT BUTTON VISUALLY
    // =========================================================

    private void SelectButton(int index)
    {
        if (
            menuButtons == null ||
            menuButtons.Length == 0
        )
            return;


        if (menuButtons[index] == null)
            return;


        if (EventSystem.current != null)
        {
            EventSystem.current
                .SetSelectedGameObject(
                    menuButtons[index].gameObject
                );
        }
    }


    // =========================================================
    // ACCEPT
    // =========================================================

    private void SelectCurrentButton()
    {
        if (
            menuButtons == null ||
            menuButtons.Length == 0
        )
            return;


        Button selectedButton =
            menuButtons[
                currentButtonIndex
            ];


        if (
            selectedButton != null &&
            selectedButton.interactable
        )
        {
            selectedButton
                .onClick
                .Invoke();
        }
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
            backButton
                .onClick
                .Invoke();
        }
    }


    // =========================================================
    // ROTATION
    // =========================================================

    private void UpdateTargetRotation()
    {
        float targetAngle =
            currentButtonIndex *
            anglePerButton;


        targetRotation =
            Quaternion.Euler(
                0f,
                0f,
                targetAngle
            );
    }
}