using UnityEngine;
using UnityEngine.InputSystem;

public class LogicaConductor : MonoBehaviour
{
    float acelerador;
    float freno;
    Vector2 giro;
    Vector2 movimiento;
    [SerializeField] float velocidadDeMovimiento;
    [SerializeField] float multiplicadorGiro;
  [SerializeField]  float multiplicadorVelocidad;
    float multiplicador;
    bool acelerando;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(freno);
        Debug.Log(acelerador);
        //Debug.Log(giro);
        if (acelerador==1)//no esta apretado
        {
            acelerando = false;
            multiplicador = -1;
            acelerador = 0;
        }
        if (acelerador==0)
        {
            multiplicador = 1;
        }
        //if (acelerando=false)
        //{

        //}
         multiplicadorVelocidad = multiplicadorVelocidad + multiplicador * Time.deltaTime ;
      
       // multiplicadorVelocidad = Mathf.Clamp(multiplicadorVelocidad,0,5);

       // Movimiento();
        Girar();
    }
    public void Acelador(InputAction.CallbackContext context)
    {
       acelerador= context.ReadValue<float>();
      
    }
    public void Freno(InputAction.CallbackContext context)
    {
        freno = context.ReadValue<float>();
    }
    public void Volante(InputAction.CallbackContext context)
    {
        giro = context.ReadValue<Vector2>();
    }
    void Movimiento()
    {
       
        if (acelerador<0 )
        {
            transform.Translate(new Vector3(0,0,1*velocidadDeMovimiento)*Time.deltaTime);
        }
    }
    void Girar()
    {
        //derecha x= 0.070 y=-0.07
        //izquierda x=-0.070 y=-0.07
        if (acelerador<0)
        {
          transform.eulerAngles = new Vector3(0, giro.x*multiplicadorGiro, 0);

        }

    }
}
