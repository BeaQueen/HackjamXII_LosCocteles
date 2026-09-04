using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CreditsNavigation : MonoBehaviour
{
    // Botón Back de la pantalla de créditos.
    [SerializeField]
    private Button backButton;


    private void Update()
    {
        // --------------------------------
        // DEBUG CON TECLADO
        // --------------------------------

        // Escape o Backspace simulan
        // el pedal de freno.
        if (Keyboard.current != null)
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame ||
                Keyboard.current.backspaceKey.wasPressedThisFrame)
            {
                GoBack();
            }
        }
    }


    // --------------------------------
    // INPUT ACTION - FRENO
    // --------------------------------

    public void Freno(InputAction.CallbackContext context)
    {
        // Solo reaccionamos cuando empieza
        // la pulsación del pedal.
        //
        // Esto evita ejecutar Back varias veces
        // mientras mantenemos el freno pisado.
        if (context.started)
        {
            GoBack();
        }
    }


    // --------------------------------
    // BACK
    // --------------------------------

    private void GoBack()
    {
        // Comprobamos que exista el botón.
        if (backButton == null)
        {
            return;
        }

        // Ejecutamos exactamente el mismo OnClick
        // que si pulsáramos el botón con el ratón.
        backButton.onClick.Invoke();
    }
}