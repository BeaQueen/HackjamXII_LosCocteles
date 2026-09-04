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

    // Velocidad a la que gira la rueda
    // hasta alcanzar la siguiente posición.
    [SerializeField]
    private float rotationSpeed = 500f;

    // Grados que separan cada botón.
    // Con 4 botones: 360 / 4 = 90 grados.
    [SerializeField]
    private float anglePerButton = 90f;

    // Sensibilidad necesaria para considerar
    // que el volante se ha girado lo suficiente.
    [SerializeField]
    [Range(0.1f, 1f)]
    private float steeringThreshold = 0.4f;

    // Índice del botón seleccionado actualmente.
    private int currentButtonIndex = 0;

    // Rotación hacia la que queremos mover la rueda.
    private Quaternion targetRotation;

    // Evita que mantener el volante girado
    // cambie de opción continuamente.
    private bool steeringReturnedToCenter = true;


    private void Start()
    {
        // Si tenemos botones configurados...
        if (menuButtons.Length > 0)
        {
            // Seleccionamos el primero.
            SelectButton(currentButtonIndex);
        }

        // Calculamos la rotación inicial.
        UpdateTargetRotation();
    }


    private void Update()
    {
        // --------------------------------
        // DEBUG CON TECLADO
        // --------------------------------

        // Flecha derecha:
        // movemos la selección hacia el lado derecho.
        if (Keyboard.current != null &&
            Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            MoveSelection(-1);
        }

        // Flecha izquierda:
        // movemos la selección hacia el lado izquierdo.
        if (Keyboard.current != null &&
            Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            MoveSelection(1);
        }


        // --------------------------------
        // CONTROL CON VOLANTE
        // --------------------------------

        if (Joystick.current != null)
        {
            // Leemos el eje horizontal del volante.
            float steering =
                Joystick.current.stick.x.ReadValue();

            // Si el volante vuelve aproximadamente al centro,
            // permitimos realizar otra selección.
            if (Mathf.Abs(steering) < 0.2f)
            {
                steeringReturnedToCenter = true;
            }

            // Solo navegamos si el volante
            // ha vuelto antes al centro.
            if (steeringReturnedToCenter)
            {
                // Volante hacia la derecha.
                if (steering > steeringThreshold)
                {
                    MoveSelection(-1);

                    // Evitamos múltiples cambios
                    // mientras seguimos girando.
                    steeringReturnedToCenter = false;
                }

                // Volante hacia la izquierda.
                else if (steering < -steeringThreshold)
                {
                    MoveSelection(1);

                    // Evitamos múltiples cambios
                    // mientras seguimos girando.
                    steeringReturnedToCenter = false;
                }
            }
        }


        // --------------------------------
        // ROTACIÓN VISUAL DE LA RUEDA
        // --------------------------------

        // Giramos suavemente hacia la posición objetivo.
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
        // según la dirección recibida.
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

        // Actualizamos la rotación de la rueda
        // para que el botón seleccionado quede arriba.
        UpdateTargetRotation();
    }


    private void SelectButton(int index)
    {
        // Indicamos al EventSystem
        // qué botón está seleccionado.
        EventSystem.current.SetSelectedGameObject(
            menuButtons[index].gameObject
        );
    }


    private void UpdateTargetRotation()
    {
        // Calculamos la rotación necesaria
        // para colocar el botón seleccionado arriba.
        float targetAngle =
            currentButtonIndex * anglePerButton;

        targetRotation =
            Quaternion.Euler(0f, 0f, targetAngle);
    }
}