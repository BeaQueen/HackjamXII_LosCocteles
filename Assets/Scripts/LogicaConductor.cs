using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static Interfaces;

public class LogicaConductor : MonoBehaviour
{
   public float acelerador;
    float freno;
    Vector2 giro;
    [SerializeField] float rotacionY;
    [SerializeField] float rotacionSumando;
    [SerializeField] Transform ruedasAGirar;
    [SerializeField] Transform ruedasAGirarDerrape;
    [SerializeField] Transform orientacionCamaraAyuda;
    [SerializeField] float velocidadDeMovimiento;
    [SerializeField] float multiplicadorGiro;
 public  float revoluciones;
    [SerializeField] float multiplicadorVelocidad;
    float multiplicadorVelocidadMaxima=10;
    [SerializeField] float offsetDerrape;
    

   public float multiplicador;
    float multiplicadorFreno;
    public bool acelerando;
    [SerializeField] bool frenando;
   public bool puedoGirar;
    [SerializeField] bool puedoGirarDerrape;
    Vector3 nuevoAngulo;
    [SerializeField] Image fillAmountRevoluciones;
    float velocidadRotacionRef;
    [SerializeField] float smothGiro;
    [SerializeField] float smothGiroDerrape;
    [SerializeField] float smothGiroImpulsoAyuda;
    [SerializeField] PolvoCoche scriptPolvoCoche;
    [SerializeField] Animator animCoche;

    [SerializeField] GameObject camaraVelocidadBaja;
    [SerializeField] GameObject camaraVelocidadAlta;
    [SerializeField] GameObject camaraPrimeraPersona;
    [SerializeField] GameObject camaraTerceraPersona;

    [SerializeField] GameObject lineasDeVelocidadMaxima;
    [SerializeField] GameObject lineasDeVelocidadTurbo;
    [SerializeField] bool impulso;
   [SerializeField] bool ayudaOrientacion;
    Vector3 direccionImpulso;
    float fuerzaImpulso;
    CharacterController controlador;
    [SerializeField] GameObject camara;
   [SerializeField] Vector3 direccionLocal;
   public bool puedoPisarAcelerador;
    public bool orientarPorGolpe;
    public Vector3 direccionOrientacionPorGolpe;
    [SerializeField] List<GameObject> listaDeCoches;
    [SerializeField] GameObject queCocheSoy;
    [SerializeField] int indiceCoche;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controlador = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(freno);
        Debug.Log(acelerador);
        Debug.Log(giro.x);
        if (giro.x>-0.01&&giro.x<0.01)
        {
            Orientarse(transform.forward, 0, ruedasAGirar.transform);
            Orientarse(transform.forward, 0, ruedasAGirarDerrape.transform);
            animCoche.SetBool("DerrapeDerecha", false);
            animCoche.SetBool("DerrapeIzquierda", false);
          //  animCoche.SetTrigger("llegaA0");
            puedoGirar = false;
            puedoGirarDerrape = false;
        }
        else
        {
            puedoGirar= true;

        }

         if (giro.x < -0.3 || giro.x > 0.3)//si paso del limite de giro
        {
           puedoGirarDerrape = true;
            if (scriptPolvoCoche != null)
            {
                scriptPolvoCoche.ActivarParticulasPolvo();

            }
        }
        else if (giro.x > -0.3 || giro.x < 0.3)
        {
            puedoGirarDerrape = false;
            if (scriptPolvoCoche != null)
            {
                scriptPolvoCoche.DesactivarParticulasPolvo();

            }

        }
       

        rotacionY = ruedasAGirar.transform.rotation.eulerAngles.y;

       LogicaAcelerador();
        LogicaFreno();


        if (acelerando == true)
        {
           revoluciones = revoluciones + 5*multiplicador * Time.deltaTime ;
           

        }
        else
        {
           
            if (frenando==false)
            {
              revoluciones =revoluciones -2 * Time.deltaTime;

            }
            else
            {
                revoluciones = revoluciones - 5 *multiplicadorFreno* Time.deltaTime;
            }
        }


       
         revoluciones = Mathf.Clamp(revoluciones,0,multiplicadorVelocidadMaxima);
        multiplicadorFreno = Mathf.Clamp(multiplicadorFreno, 0, 10);

        CamarasVelocidad();
      


        Movimiento();
         Girar();
        // FillAmount();
        if (orientarPorGolpe)
        {
            OrientarsePorGolpe(direccionOrientacionPorGolpe, 0.1f, transform);
        }
    }
    public void ActivarCoche(int indice)
    {
        //for (int i = 0; i < listaDeCoches.Count; i++)
        //{
        //    if (i==indice)
        //    {
        //        if (!listaDeCoches[i])
        //        listaDeCoches[i].SetActive(true);
        //        queCocheSoy = listaDeCoches[i];
                
        //    }
        //    else
        //    {
        //        listaDeCoches[i].SetActive(false);

        //    }
        //}
    }
    public void Acelador(InputAction.CallbackContext context)
    {
        if (puedoPisarAcelerador)
        {
         acelerador= context.ReadValue<float>();

        }
      
    }
    public void Freno(InputAction.CallbackContext context)
    {
        freno = context.ReadValue<float>();
    }
    public void Volante(InputAction.CallbackContext context)
    {
        giro = context.ReadValue<Vector2>();
    }
    public void GatilloIzquierdo(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            indiceCoche --;
            //if (indiceCoche < 0)
            //{
            //    indiceCoche = listaDeCoches.Count;
            //}

        }
    }
    public void GatilloDerecho(InputAction.CallbackContext context)
    {
        Debug.Log("presiono");
        if (context.performed)
        {
            indiceCoche ++;
            //if (indiceCoche>listaDeCoches.Count)
            //{
            //    indiceCoche = 0;
            //}

        }
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
        if (multiplicador>5)
        {
            lineasDeVelocidadMaxima.SetActive(true);
        }
        else
        {
            lineasDeVelocidadMaxima.SetActive(false);
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
            multiplicadorFreno = multiplicadorFreno + 10 * Time.deltaTime * Mathf.Abs(freno);
        }

    }
    void FillAmount()
    {
        fillAmountRevoluciones.fillAmount = revoluciones / multiplicadorVelocidadMaxima;
       
    }
    void CamarasVelocidad()
    {
        if (revoluciones>5)
        {
            camaraVelocidadAlta.SetActive(true);
        }
        else if(revoluciones < 5)
        {
            camaraVelocidadAlta.SetActive(false);
        }
    }
    public void ReducirRevoluciones()
    {
        revoluciones=0;//quitamos revoluciones
        acelerador = 0;//acelerador a 0
      
       
       
    }
   
   public void Orientarse(Vector3 direccion, float Smoth, Transform objetoAOrientar)
    {

        //Arcotangente,convierte la rotacion en grados ,para saber que rotacion ponerle a mi personaje
        float angulo = Mathf.Atan2(direccion.x * 1, direccion.z * 1) * Mathf.Rad2Deg;//el calculo me lo dan en radios ...Y LO TENGO QUE CONVERTIR A RADIANES

      
        
            float anguloSuave = Mathf.SmoothDampAngle(objetoAOrientar.eulerAngles.y, angulo, ref velocidadRotacionRef, Smoth);//vamos a crear una interpolacion entre el angulo al que estamos mirando y al angulo hacia el que vamos a mirar , con una velocidad de rotacion y un  Smootheado


            objetoAOrientar.eulerAngles = new Vector3(0, anguloSuave, 0);

        





    }
    public void OrientarsePorGolpe(Vector3 direccion, float Smoth, Transform objetoAOrientar)
    {

        //Arcotangente,convierte la rotacion en grados ,para saber que rotacion ponerle a mi personaje
        float angulo = Mathf.Atan2(direccion.x * 1, direccion.z * 1) * Mathf.Rad2Deg;//el calculo me lo dan en radios ...Y LO TENGO QUE CONVERTIR A RADIANES



        float anguloSuave = Mathf.SmoothDampAngle(objetoAOrientar.eulerAngles.y, angulo, ref velocidadRotacionRef, Smoth);//vamos a crear una interpolacion entre el angulo al que estamos mirando y al angulo hacia el que vamos a mirar , con una velocidad de rotacion y un  Smootheado


        objetoAOrientar.eulerAngles = new Vector3(0, anguloSuave, 0);







    }
    public void Impulso(Vector3 direccionimpulsoo, float fuerzaImpulsoo)
    {
        impulso = true;
        direccionImpulso = direccionimpulsoo;
        fuerzaImpulso=fuerzaImpulsoo;
       
       
    }
    void Movimiento()
    {
        if(impulso==false)
        {
            //  transform.Translate(new Vector3(0,0,multiplicadorVelocidad*revoluciones)*Time.deltaTime);
            direccionLocal = camara.transform.forward * -giro.y + camara.transform.right * giro.x;//cogemos el frente de la camara en torno al movimiento del joystick izquierdo
            direccionLocal.y = 0;//cancelamos subidas y bajadas con la camara
            controlador.Move((new Vector3(direccionLocal.x, 0, direccionLocal.z).normalized * multiplicadorVelocidad * revoluciones)*Time.deltaTime ) ;
           // controlador.


        }
        else
        {
            //revoluciones = 0;
            //transform.Translate(direccionImpulso * fuerzaImpulso * Time.deltaTime);
            //Invoke("DesactivarImpulso", 2f);
        }


    }
    void DesactivarImpulso()
    {
        impulso=false;
    }
    void Girar()
    {
        //derecha x= 0.070 y=-0.07
        //izquierda x=-0.070 y=-0.07

        if (puedoGirar)//si puedo girar porq no esta cerca del offset del volante...
        {
          
          ruedasAGirar.eulerAngles = new Vector3(0, giro.x * multiplicadorGiro+transform.rotation.eulerAngles.y, 0);//hay giro
           // orientacionCamaraAyuda.eulerAngles= new Vector3(0, giro.x * multiplicadorGiro + transform.rotation.eulerAngles.y, 0)*-1;//hay giro
                                                                                                                                 //  ruedasAGirar.transform.eulerAngles = new Vector3(0, Mathf.Clamp(ruedasAGirar.transform.eulerAngles.y, -90, 90));
                                                                                                                                 //  ruedasAGirarDerrape.eulerAngles = new Vector3(0, giro.x * multiplicadorGiro+offsetDerrape + transform.rotation.eulerAngles.y, 0);//hay giro

            if (revoluciones > 0)//si hay velocidad...
            {
                if (puedoGirarDerrape)
                {
                    //   Orientarse(ruedasAGirar.transform.forward+ ruedasAGirarDerrape.transform.forward, smothGiro, transform);//orientate segun esa direccion
                    // Orientarse(ruedasAGirarDerrape.transform.forward, smothGiro, ruedasAGirar.transform);
                    if (giro.x>0)//derecha..
                    {
                         animCoche.SetBool("DerrapeDerecha",true);

                    }
                    else if(giro.x<0)
                    {
                        animCoche.SetBool("DerrapeIzquierda", true);
                    }
                    if (ayudaOrientacion==false)
                    {
                     Orientarse(ruedasAGirar.transform.forward, smothGiroDerrape, transform);//orientate segun esa direccion

                    }
                }
                else
                {
                    animCoche.SetBool("DerrapeDerecha", false);
                    animCoche.SetBool("DerrapeIzquierda", false);
                    if (ayudaOrientacion==false)
                    {
                      Orientarse(ruedasAGirar.transform.forward, smothGiro, transform);//orientate segun esa direccion

                    }
                }


                

            }
           

        }

    }
    public void OrientacionAyuda()
    {
        //if (revoluciones > 4)
        //{
        //    ayudaOrientacion = true;
        //    Orientarse(orientacionCamaraAyuda.transform.forward, smothGiroImpulsoAyuda, transform);//orientate segun esa direccion
        //    Invoke("TerminarOrientacioAyuda", 0.1f);

        //}
    }
    void TerminarOrientacioAyuda()
    {

        ayudaOrientacion = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.transform.gameObject.name);
        if (other.gameObject.TryGetComponent(out IColision objetoConQueColisiono))
        {
            objetoConQueColisiono.Colision(gameObject, new Vector3(0,0,0));
        }
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Debug.Log(hit.transform.gameObject.name);
        if (hit.gameObject.TryGetComponent(out IColision objetoConQueColisiono))
        {
            
            objetoConQueColisiono.Colision(gameObject,hit.normal);
        }
    }

}
