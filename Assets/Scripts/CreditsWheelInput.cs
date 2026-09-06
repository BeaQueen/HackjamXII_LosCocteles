using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CreditsWheelInput : MonoBehaviour
{
    // =========================================================
    // UI
    // =========================================================

    [Header("UI")]
    [SerializeField] private Button backButton;


    // =========================================================
    // INPUT
    // =========================================================

    [Header("Wheel Input")]
    [SerializeField] private InputActionReference brakeAction;


    // =========================================================
    // PEDAL
    // =========================================================

    [Header("Pedal Settings")]

    [SerializeField]
    private float pedalPressedThreshold = 0.8f;

    [SerializeField]
    private float pedalReleasedThreshold = 0.95f;


    // =========================================================
    // INTERNAL
    // =========================================================

    private bool brakeReady = false;
    private bool brakeWasPressed = false;


    // =========================================================
    // UNITY
    // =========================================================

    private void OnEnable()
    {
        if (brakeAction != null)
        {
            brakeAction.action.Enable();
        }
    }


    private void OnDisable()
    {
        if (brakeAction != null)
        {
            brakeAction.action.Disable();
        }
    }


    private void Update()
    {
        ReadBrake();


        // Debug con teclado
        if (
            Keyboard.current != null &&
            (
                Keyboard.current.escapeKey.wasPressedThisFrame ||
                Keyboard.current.backspaceKey.wasPressedThisFrame
            )
        )
        {
            GoBack();
        }
    }


    // =========================================================
    // BRAKE
    // =========================================================

    private void ReadBrake()
    {
        if (brakeAction == null)
            return;


        float rawValue =
            brakeAction.action.ReadValue<float>();


        // Esperamos primero a detectar
        // el pedal suelto.
        if (!brakeReady)
        {
            if (rawValue >= pedalReleasedThreshold)
            {
                brakeReady = true;
                brakeWasPressed = false;
            }

            return;
        }


        bool pressed =
            rawValue < pedalPressedThreshold;


        // Solo una vez por cada pisada
        if (
            pressed &&
            !brakeWasPressed
        )
        {
            GoBack();
        }


        brakeWasPressed = pressed;
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
            Debug.Log("CREDITS: BACK");

            backButton.onClick.Invoke();
        }
    }
}