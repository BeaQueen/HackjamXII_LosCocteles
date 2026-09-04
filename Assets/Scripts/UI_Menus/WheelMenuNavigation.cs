using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class WheelMenuNavigation : MonoBehaviour
{
    // Botones del menú colocados en orden:
    // Play -> Exit -> Settings -> Credits.
    [SerializeField]
    private Button[] menuButtons;

    // Botón Back de esta pantalla.
    // En el Main Menu puede quedarse vacío.
    [SerializeField]
    private Button backButton;

    // RectTransform que contiene toda la rueda.
    // Este objeto será el que rote visualmente.
    [SerializeField]
    private RectTransform menuWheel;

    // Velocidad visual a la que gira la rueda.
    [SerializeField]
    private float rotationSpeed = 500f;

    // Grados que separan cada botón.
    // Con cuatro botones: 360 / 4 = 90 grados.
    [SerializeField]
    private float anglePerButton = 90f;

    // Cantidad mínima de giro necesaria
    // para considerar que el volante está girando.
    [SerializeField]
    private float steeringThreshold = 0.015f;

    // Valor a partir del cual consideramos
    // que un pedal está siendo pulsado.
    //
    // Por los valores del volante de vuestro programador,
    // parece que los pedales pasan a valores negativos
    // cuando se pisan.
    [SerializeField]
    private float pedalPressedThreshold = -0.1f;

    // Tiempo que esperamos antes de empezar
    // a repetir el movimiento al mantener el volante girado.
    [SerializeField]
    private float initialRepeatDelay = 0.4f;

    // Tiempo entre cambios mientras
    // mantenemos el volante girado.
    [SerializeField]
    private float repeatInterval = 0.2f;

    // Valor recibido de la Input Action del volante.
    private Vector2 giro;

    // Estado del pedal del acelerador.
    private float acelerador;

    // Estado del pedal de freno.
    private float freno;

    // Nos permite detectar solamente
    // el momento en el que se pisa el acelerador.
    private bool aceleradorPulsado = false;

    // Nos permite detectar solamente
    // el momento en el que se pisa el freno.
    private bool frenoPulsado = false;

    // Índice del botón seleccionado actualmente.
    private int currentButtonIndex = 0;

    // Rotación hacia la que queremos mover la rueda.
    private Quaternion targetRotation;

    // Dirección mantenida actualmente:
    //
    // -1 = derecha
    //  1 = izquierda
    //  0 = ninguna
    private int heldDirection = 0;

    // Momento en el que podremos realizar
    // el siguiente movimiento repetido.
    private float nextRepeatTime = 0f;


    private void Start()
    {
        // Si tenemos botones configurados,
        // seleccionamos el primero.
        if (menuButtons.Length > 0)
        {
            SelectButton(currentButtonIndex);
        }

        // Establecemos la rotación inicial.
        UpdateTargetRotation();
    }


    private void Update()
    {
        // Dirección recibida este frame.
        int inputDirection = 0;


        // ==================================================
        // DEBUG CON TECLADO
        // ==================================================

        if (Keyboard.current != null)
        {
            // Flecha derecha.
            if (Keyboard.current.rightArrowKey.isPressed)
            {
                inputDirection = -1;
            }

            // Flecha izquierda.
            else if (Keyboard.current.leftArrowKey.isPressed)
            {
                inputDirection = 1;
            }


            // ENTER activa el botón seleccionado.
            if (Keyboard.current.enterKey.wasPressedThisFrame)
            {
                SelectCurrentButton();
            }


            // ESCAPE o BACKSPACE actúan como el pedal de freno
            // y pulsan el botón Back.
            if (Keyboard.current.escapeKey.wasPressedThisFrame ||
                Keyboard.current.backspaceKey.wasPressedThisFrame)
            {
                GoBack();
            }
        }


        // ==================================================
        // VOLANTE
        // ==================================================

        // Solo utilizamos el volante si no estamos
        // usando las flechas del teclado.
        if (inputDirection == 0)
        {
            // Giro a la derecha.
            if (giro.x > steeringThreshold)
            {
                inputDirection = -1;
            }

            // Giro a la izquierda.
            else if (giro.x < -steeringThreshold)
            {
                inputDirection = 1;
            }
        }


        // ==================================================
        // NAVEGACIÓN POR LA RUEDA
        // ==================================================

        // Si hemos dejado de girar...
        if (inputDirection == 0)
        {
            // Reiniciamos la dirección mantenida.
            heldDirection = 0;
        }

        // Si acabamos de empezar a girar
        // o hemos cambiado de dirección...
        else if (inputDirection != heldDirection)
        {
            // Guardamos la nueva dirección.
            heldDirection = inputDirection;

            // Movemos inmediatamente una opción.
            MoveSelection(heldDirection);

            // Esperamos antes de empezar
            // la repetición automática.
            nextRepeatTime =
                Time.unscaledTime + initialRepeatDelay;
        }

        // Si seguimos manteniendo la misma dirección...
        else if (Time.unscaledTime >= nextRepeatTime)
        {
            // Seguimos rotando por las opciones.
            MoveSelection(heldDirection);

            // Programamos el siguiente movimiento.
            nextRepeatTime =
                Time.unscaledTime + repeatInterval;
        }


        // ==================================================
        // ROTACIÓN VISUAL DE LA RUEDA
        // ==================================================

        // Giramos suavemente hacia
        // la posición correspondiente.
        menuWheel.localRotation =
            Quaternion.RotateTowards(
                menuWheel.localRotation,
                targetRotation,
                rotationSpeed * Time.unscaledDeltaTime
            );
    }


    // ==================================================
    // INPUT ACTION - VOLANTE
    // ==================================================

    // Esta función debe estar conectada
    // a la misma Input Action "Volante"
    // que utiliza vuestro LogicaConductor.
    public void Volante(InputAction.CallbackContext context)
    {
        giro = context.ReadValue<Vector2>();
    }


    // ==================================================
    // INPUT ACTION - ACELERADOR
    // ==================================================

    // Esta función debe estar conectada
    // a la Input Action del acelerador.
    public void Acelador(InputAction.CallbackContext context)
    {
        // Leemos el valor recibido del pedal.
        acelerador = context.ReadValue<float>();

        // Comprobamos si el pedal está siendo pisado.
        bool pedalAhoraPulsado =
            acelerador < pedalPressedThreshold;


        // Solo ejecutamos la selección
        // justo en el momento de pisar el pedal.
        //
        // Así evitamos activar varias veces el botón
        // mientras mantenemos el pedal pisado.
        if (pedalAhoraPulsado && !aceleradorPulsado)
        {
            SelectCurrentButton();
        }


        // Guardamos el estado actual
        // para compararlo en la siguiente lectura.
        aceleradorPulsado = pedalAhoraPulsado;
    }


    // ==================================================
    // INPUT ACTION - FRENO
    // ==================================================

    // Esta función debe estar conectada
    // a la Input Action del freno.
    public void Freno(InputAction.CallbackContext context)
    {
        // Leemos el valor recibido del pedal.
        freno = context.ReadValue<float>();

        // Comprobamos si el pedal está pisado.
        bool pedalAhoraPulsado =
            freno < pedalPressedThreshold;


        // Solo ejecutamos Back
        // en el momento de empezar a pisar.
        if (pedalAhoraPulsado && !frenoPulsado)
        {
            GoBack();
        }


        // Guardamos el estado actual.
        frenoPulsado = pedalAhoraPulsado;
    }


    // ==================================================
    // SELECCIONAR
    // ==================================================

    private void SelectCurrentButton()
    {
        // Comprobamos que haya botones
        // y que el índice sea válido.
        if (menuButtons.Length == 0)
        {
            return;
        }

        // Ejecutamos exactamente el mismo OnClick
        // que tendría el botón si lo pulsáramos con el ratón.
        menuButtons[currentButtonIndex].onClick.Invoke();
    }


    // ==================================================
    // BACK
    // ==================================================

    private void GoBack()
    {
        // Si esta pantalla no tiene botón Back,
        // simplemente no hacemos nada.
        //
        // Esto nos permite dejar Back vacío
        // en el Main Menu.
        if (backButton == null)
        {
            return;
        }

        // Ejecutamos el OnClick del botón Back.
        backButton.onClick.Invoke();
    }


    // ==================================================
    // CAMBIO DE SELECCIÓN
    // ==================================================

    private void MoveSelection(int direction)
    {
        // Cambiamos el índice.
        currentButtonIndex += direction;


        // Si pasamos del último botón,
        // volvemos al primero.
        if (currentButtonIndex >= menuButtons.Length)
        {
            currentButtonIndex = 0;
        }

        // Si retrocedemos desde el primero,
        // saltamos al último.
        else if (currentButtonIndex < 0)
        {
            currentButtonIndex = menuButtons.Length - 1;
        }


        // Seleccionamos el nuevo botón.
        SelectButton(currentButtonIndex);

        // Actualizamos la rotación.
        UpdateTargetRotation();
    }


    private void SelectButton(int index)
    {
        // Marcamos el botón como seleccionado
        // dentro del EventSystem.
        EventSystem.current.SetSelectedGameObject(
            menuButtons[index].gameObject
        );
    }


    private void UpdateTargetRotation()
    {
        // Calculamos el ángulo necesario
        // para colocar la opción seleccionada arriba.
        float targetAngle =
            currentButtonIndex * anglePerButton;

        targetRotation =
            Quaternion.Euler(0f, 0f, targetAngle);
    }
}