using UnityEngine;
using UnityEngine.InputSystem;

public class Cambio : MonoBehaviour
{
    [SerializeField]
    private InputActionReference leftButtonAction;

    [SerializeField]
    private InputActionReference rightButtonAction;
    private void OnEnable()
    {
        leftButtonAction.action.performed += gatilloIzquierdo; ;
        rightButtonAction.action.performed += gatilloDerecho; ;


        leftButtonAction.action.Enable();
        rightButtonAction.action.Enable();

    }

    private void gatilloDerecho(InputAction.CallbackContext obj)
    {
        if (obj.performed)
        {
            Debug.Log("gatillo izquierdo");

        }
    }

    private void gatilloIzquierdo(InputAction.CallbackContext obj)
    {
        Debug.Log("gatillo derecho");
    }

    private void OnDisable()
    {
        leftButtonAction.action.performed -= gatilloIzquierdo;
        rightButtonAction.action.performed -= gatilloDerecho;


        leftButtonAction.action.Disable();
        rightButtonAction.action.Disable();

    }
}
