using UnityEngine;
using UnityEngine.InputSystem;

public class GiroRuedas : MonoBehaviour
{
    Vector2 giro;
    [SerializeField] LogicaConductor scriptLogicaConductor;
    [SerializeField] float multiplicadorGiro;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(giro);
        Girar();
    }
    public void Volanteeee(InputAction.CallbackContext context)
    {
        giro = context.ReadValue<Vector2>();
    }
    void Girar()
    {
        //derecha x= 0.070 y=-0.07
        //izquierda x=-0.070 y=-0.07
            transform.eulerAngles = new Vector3(0, giro.x *  multiplicadorGiro, 0);//hay giro
        //if (scriptLogicaConductor.multiplicadorVelocidad > 0)//si hay velocidad...
        //{

        //}

    }
}
