using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class WheelMenuNavigation : MonoBehaviour
{
    // Botones del menú, colocados en orden horario:
    // Play -> Exit -> Settings -> Credits.
    [SerializeField]
    private Button[] menuButtons;

    // RectTransform que contiene toda la rueda.
    // Este objeto será el que rote visualmente.
    [SerializeField]
    private RectTransform menuWheel;

    // Velocidad visual a la que gira la rueda.
    [SerializeField]
    private float rotationSpeed = 500f;

    // Grados entre cada botón.
    // Con 4 botones serían 90 grados.
    [SerializeField]
    private float anglePerButton = 90f;

    // Cantidad mínima de giro del volante
    // necesaria para empezar a navegar.
    [SerializeField]
    [Range(0.1f, 1f)]
    private float steeringThreshold = 0.4f;

    // Tiempo que esperamos antes de empezar
    // a repetir el movimiento al mantener pulsado.
    [SerializeField]
    private float initialRepeatDelay = 0.5f;

    // Tiempo entre cada movimiento una vez
    // estamos manteniendo la dirección.
    [SerializeField]
    private float repeatInterval = 0.2f;

    // Índice del botón seleccionado actualmente.
    private int currentButtonIndex = 0;

    // Rotación objetivo de la rueda.
    private Quaternion targetRotation;

    // Dirección que estamos manteniendo actualmente.
    // -1 = derecha
    //  1 = izquierda
    //  0 = ninguna
    private int heldDirection = 0;

    // Momento en el que podremos realizar
    // el siguiente movimiento repetido.
    private float nextRepeatTime = 0f;


    private void Start()
    {
        // Seleccionamos el primer botón al iniciar.
        if (menuButtons.Length > 0)
        {
            SelectButton(currentButtonIndex);
        }

        // Calculamos la rotación inicial.
        UpdateTargetRotation();
    }


    private void Update()
    {
        // Dirección que estamos recibiendo
        // en este frame.
        int inputDirection = 0;


        // --------------------------------
        // DEBUG CON TECLADO
        // --------------------------------

        if (Keyboard.current != null)
        {
            // Mantener flecha derecha.
            if (Keyboard.current.rightArrowKey.isPressed)
            {
                inputDirection = -1;
            }

            // Mantener flecha izquierda.
            else if (Keyboard.current.leftArrowKey.isPressed)
            {
                inputDirection = 1;
            }
        }


        // --------------------------------
        // CONTROL CON VOLANTE
        // --------------------------------

        // Solo usamos el volante si no estamos
        // utilizando el teclado en este momento.
        if (inputDirection == 0 &&
            Joystick.current != null)
        {
            // Leemos el giro horizontal del volante.
            float steering =
                Joystick.current.stick.x.ReadValue();

            // Giro hacia la derecha.
            if (steering > steeringThreshold)
            {
                inputDirection = -1;
            }

            // Giro hacia la izquierda.
            else if (steering < -steeringThreshold)
            {
                inputDirection = 1;
            }
        }


        // --------------------------------
        // REPETICIÓN DEL MOVIMIENTO
        // --------------------------------

        // Si no estamos pulsando/girando nada...
        if (inputDirection == 0)
        {
            // Reiniciamos el estado.
            heldDirection = 0;
        }

        // Si acabamos de empezar a mantener
        // una nueva dirección...
        else if (inputDirection != heldDirection)
        {
            // Guardamos la nueva dirección.
            heldDirection = inputDirection;

            // Movemos inmediatamente una posición.
            MoveSelection(heldDirection);

            // Esperamos un poco antes
            // de empezar la repetición automática.
            nextRepeatTime =
                Time.unscaledTime + initialRepeatDelay;
        }

        // Si seguimos manteniendo la misma dirección
        // y ya ha pasado el tiempo necesario...
        else if (Time.unscaledTime >= nextRepeatTime)
        {
            // Movemos otra posición.
            MoveSelection(heldDirection);

            // Programamos el siguiente movimiento.
            nextRepeatTime =
                Time.unscaledTime + repeatInterval;
        }


        // --------------------------------
        // ROTACIÓN VISUAL
        // --------------------------------

        // Giramos suavemente la rueda
        // hacia la nueva posición.
        menuWheel.localRotation =
            Quaternion.RotateTowards(
                menuWheel.localRotation,
                targetRotation,
                rotationSpeed * Time.unscaledDeltaTime
            );
    }


    private void MoveSelection(int direction)
    {
        // Avanzamos o retrocedemos
        // por los botones.
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


        // Seleccionamos el botón correspondiente.
        SelectButton(currentButtonIndex);

        // Actualizamos hacia dónde debe girar la rueda.
        UpdateTargetRotation();
    }


    private void SelectButton(int index)
    {
        // Marcamos el botón como seleccionado
        // dentro del EventSystem de Unity.
        EventSystem.current.SetSelectedGameObject(
            menuButtons[index].gameObject
        );
    }


    private void UpdateTargetRotation()
    {
        // Calculamos el ángulo necesario
        // para dejar la opción seleccionada arriba.
        float targetAngle =
            currentButtonIndex * anglePerButton;

        targetRotation =
            Quaternion.Euler(0f, 0f, targetAngle);
    }
}