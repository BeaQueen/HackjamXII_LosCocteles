using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LogicaConductor : MonoBehaviour
{
    float acelerador;
    float freno;
    Vector2 giro;
    [SerializeField] float rotacionY;
    [SerializeField] float rotacionSumando;
    [SerializeField] Transform ruedasAGirar;
    [SerializeField] float velocidadDeMovimiento;
    [SerializeField] float multiplicadorGiro;
  [SerializeField]  float multiplicadorVelocidad;
    float multiplicadorVelocidadMaxima=10;
   
    

    float multiplicador;
    float multiplicadorFreno;
    [SerializeField] bool acelerando;
    [SerializeField] bool frenando;
    [SerializeField] bool puedoGirar;
    Vector3 nuevoAngulo;
    [SerializeField] Image fillAmountRevoluciones;
    float velocidadRotacionRef;
    [SerializeField] float smothGiro;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(freno);
        Debug.Log(acelerador);
        Debug.Log(giro);
        if (giro.x>-0.05&&giro.x<0.05)
        {
            Orientarse(transform.forward, 0, ruedasAGirar.transform);
            puedoGirar = false;
        }
        else
        {
            puedoGirar= true;
        }

        rotacionY = ruedasAGirar.transform.rotation.eulerAngles.y;

       LogicaAcelerador();
        LogicaFreno();


        if (acelerando == true)
        {
           multiplicadorVelocidad = multiplicadorVelocidad + 2*multiplicador * Time.deltaTime ;

        }
        else
        {
            if (frenando==false)
            {
              multiplicadorVelocidad = multiplicadorVelocidad -2 * Time.deltaTime;

            }
            else
            {
                multiplicadorVelocidad = multiplicadorVelocidad - 2 *multiplicadorFreno* Time.deltaTime;
            }
        }
       
         multiplicadorVelocidad = Mathf.Clamp(multiplicadorVelocidad,0,multiplicadorVelocidadMaxima);
        multiplicadorFreno = Mathf.Clamp(multiplicadorFreno, 0, 10);

      


        Movimiento();
         Girar();
        FillAmount();
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
    void LogicaAcelerador()
    {
        if (acelerador == 1)//no esta apretado
        {
            acelerando = false;
            multiplicador = 0;

        }
        else if (acelerador != 1)
        {
            acelerando = true;
            multiplicador = multiplicador + 5 * Time.deltaTime * Mathf.Abs(acelerador);
        }
    }
    void LogicaFreno()
    {
        if (freno == 1)//no esta apretado
        {
            frenando = false;
            multiplicadorFreno = 0;
        }
        else if (freno != 1)
        {
            frenando = true;
            multiplicadorFreno = multiplicadorFreno + 5 * Time.deltaTime * Mathf.Abs(freno);
        }

    }
    void FillAmount()
    {
        fillAmountRevoluciones.fillAmount = multiplicadorVelocidad / multiplicadorVelocidadMaxima;
       
    }
    void Orientarse(Vector3 direccion, float Smoth, Transform objetoAOrientar)
    {

        //Arcotangente,convierte la rotacion en grados ,para saber que rotacion ponerle a mi personaje
        float angulo = Mathf.Atan2(direccion.x * 1, direccion.z * 1) * Mathf.Rad2Deg;//el calculo me lo dan en radios ...Y LO TENGO QUE CONVERTIR A RADIANES

      
        
            float anguloSuave = Mathf.SmoothDampAngle(objetoAOrientar.eulerAngles.y, angulo, ref velocidadRotacionRef, Smoth);//vamos a crear una interpolacion entre el angulo al que estamos mirando y al angulo hacia el que vamos a mirar , con una velocidad de rotacion y un  Smootheado


            objetoAOrientar.eulerAngles = new Vector3(0, anguloSuave, 0);

        





    }
    void Movimiento()
    {
            transform.Translate(new Vector3(0,0,1*multiplicadorVelocidad)*Time.deltaTime);
       
      
    }
    void Girar()
    {
        //derecha x= 0.070 y=-0.07
        //izquierda x=-0.070 y=-0.07

        if (puedoGirar)//si puedo girar porq no esta cerca del offset del volante...
        {
          
          ruedasAGirar.eulerAngles = new Vector3(0, giro.x * multiplicadorGiro+transform.rotation.eulerAngles.y, 0);//hay giro
            if (multiplicadorVelocidad > 0)//si hay velocidad...
            {
                Orientarse(ruedasAGirar.transform.forward, smothGiro, transform);//orientate segun esa direccion

            }

        }

    }
}
