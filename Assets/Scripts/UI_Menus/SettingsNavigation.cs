using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class SettingsNavigation : MonoBehaviour
{
    // Dropdown que contiene los idiomas disponibles.
    [SerializeField]
    private TMP_Dropdown languageDropdown;

    // Botón Back de Settings.
    [SerializeField]
    private Button backButton;

    // Giro mínimo necesario para detectar
    // movimiento del volante.
    [SerializeField]
    private float steeringThreshold = 0.015f;

    // Tiempo antes de empezar a repetir
    // si mantenemos el volante girado.
    [SerializeField]
    private float initialRepeatDelay = 0.4f;

    // Tiempo entre movimientos mientras
    // mantenemos el volante girado.
    [SerializeField]
    private float repeatInterval = 0.2f;

    // Valor recibido del volante.
    private Vector2 giro;

    // Dirección que estamos manteniendo.
    //
    // -1 = subir
    //  1 = bajar
    //  0 = nada
    private int heldDirection = 0;

    // Momento en el que podremos volver
    // a cambiar de opción.
    private float nextRepeatTime = 0f;


    private void Update()
    {
        // Dirección detectada este frame.
        int inputDirection = 0;


        // ==================================================
        // DEBUG CON TECLADO
        // ==================================================

        if (Keyboard.current != null)
        {
            // Flecha derecha:
            // subimos una opción en el Dropdown.
            if (Keyboard.current.rightArrowKey.isPressed)
            {
                inputDirection = -1;
            }

            // Flecha izquierda:
            // bajamos una opción.
            else if (Keyboard.current.leftArrowKey.isPressed)
            {
                inputDirection = 1;
            }


            // Escape o Backspace:
            // simulamos el freno.
            if (Keyboard.current.escapeKey.wasPressedThisFrame ||
                Keyboard.current.backspaceKey.wasPressedThisFrame)
            {
                GoBack();
            }
        }


        // ==================================================
        // VOLANTE
        // ==================================================

        // Solo leemos el volante si no estamos
        // utilizando el teclado.
        if (inputDirection == 0)
        {
            // Girar a la derecha = subir.
            if (giro.x > steeringThreshold)
            {
                inputDirection = -1;
            }

            // Girar a la izquierda = bajar.
            else if (giro.x < -steeringThreshold)
            {
                inputDirection = 1;
            }
        }


        // ==================================================
        // NAVEGACIÓN DEL DROPDOWN
        // ==================================================

        // Si hemos dejado de girar...
        if (inputDirection == 0)
        {
            // Reiniciamos el estado.
            heldDirection = 0;
        }

        // Si empezamos a girar
        // o cambiamos de dirección...
        else if (inputDirection != heldDirection)
        {
            // Guardamos la dirección actual.
            heldDirection = inputDirection;

            // Cambiamos inmediatamente de opción.
            ChangeDropdownOption(heldDirection);

            // Esperamos antes de empezar
            // la repetición automática.
            nextRepeatTime =
                Time.unscaledTime + initialRepeatDelay;
        }

        // Si seguimos manteniendo el giro...
        else if (Time.unscaledTime >= nextRepeatTime)
        {
            // Seguimos cambiando opciones.
            ChangeDropdownOption(heldDirection);

            // Programamos el siguiente cambio.
            nextRepeatTime =
                Time.unscaledTime + repeatInterval;
        }
    }


    // ==================================================
    // INPUT ACTION - VOLANTE
    // ==================================================

    public void Volante(InputAction.CallbackContext context)
    {
        // Guardamos el Vector2 recibido
        // de vuestra Input Action del volante.
        giro = context.ReadValue<Vector2>();
    }


    // ==================================================
    // INPUT ACTION - FRENO
    // ==================================================

    public void Freno(InputAction.CallbackContext context)
    {
        // Ejecutamos Back solamente
        // al comenzar a pulsar el pedal.
        if (context.started)
        {
            GoBack();
        }
    }


    // ==================================================
    // CAMBIO DEL DROPDOWN
    // ==================================================

    private void ChangeDropdownOption(int direction)
    {
        // Comprobamos que exista el Dropdown
        // y que tenga opciones.
        if (languageDropdown == null ||
            languageDropdown.options.Count == 0)
        {
            return;
        }


        // Calculamos la nueva posición.
        int newValue =
            languageDropdown.value + direction;


        // Evitamos salirnos de los límites.
        //
        // Si estamos arriba del todo y volvemos
        // a intentar subir, nos quedamos arriba.
        //
        // Lo mismo abajo.
        newValue = Mathf.Clamp(
            newValue,
            0,
            languageDropdown.options.Count - 1
        );


        // Cambiamos la opción seleccionada.
        //
        // IMPORTANTE:
        // usamos .value directamente para que
        // se ejecute onValueChanged y cambie
        // realmente el idioma.
        languageDropdown.value = newValue;

        // Actualizamos el texto mostrado.
        languageDropdown.RefreshShownValue();
    }


    // ==================================================
    // BACK
    // ==================================================

    private void GoBack()
    {
        // Si no tenemos Back configurado,
        // no hacemos nada.
        if (backButton == null)
        {
            return;
        }

        // Ejecutamos el OnClick del botón Back.
        backButton.onClick.Invoke();
    }
}