using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static Interfaces;

public class LogicaConductor : TipoDeCoche
{
    [Header("Seleccion de coche")]
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
   public GameObject queCocheSoy;
    [SerializeField] int indiceCoche;
   
<<<<<<< Updated upstream
<<<<<<< Updated upstream
   
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes



    // Start is called once before the first execution of Update after the MonoBehaviour is created

   
    void Start()
    {
        controlador = GetComponent<CharacterController>();
<<<<<<< Updated upstream
<<<<<<< Updated upstream

        if (listaDeCoches != null && listaDeCoches.Count > 0)
        {
            indiceCoche = Mathf.Clamp(indiceCoche, 0, listaDeCoches.Count - 1);
            ActivarCoche(indiceCoche);
        }
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
    }
  
    // Update is called once per frame
    void Update()
    {
<<<<<<< Updated upstream
        if (gameObject.CompareTag("Player"))
=======
        //Debug.Log(freno);
        //Debug.Log(acelerador);
        //Debug.Log(giro.x);
        if (giro.x>-0.01&&giro.x<0.01)
>>>>>>> Stashed changes
        {
            //Debug.Log(freno);
            //Debug.Log(acelerador);
            //Debug.Log(giro.x);
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
      
    }
   
   
    public void ActivarCoche(int indice)
    {
        Debug.Log(
        "ActivarCoche llamado con índice: " +
        indice
    );

        if (listaDeCoches == null || listaDeCoches.Count == 0)
        {
            Debug.LogError("ListaDeCoches está vacía");
            return;
        }
        if (listaDeCoches == null || listaDeCoches.Count == 0)
            return;

        // Hacemos loop en la lista
        if (indice < 0)
        {
            indice = listaDeCoches.Count - 1;
        }
        else if (indice >= listaDeCoches.Count)
        {
            indice = 0;
        }

        indiceCoche = indice;

        for (int i = 0; i < listaDeCoches.Count; i++)
        {
            bool esCocheSeleccionado = i == indiceCoche;

            listaDeCoches[i].SetActive(esCocheSeleccionado);

            if (esCocheSeleccionado)
            {
                queCocheSoy = listaDeCoches[i];
                if (i==0)
                {
                    tipoDeCoche=Coche.Presente;
                }
                else if (i==1)
                {
                    tipoDeCoche=Coche.Futuro;
                }
                else if(i==2)
                {
                    tipoDeCoche=Coche.Pasado;
                }
            }
        }

        Debug.Log(
            "Coche seleccionado: " +
            queCocheSoy.name +
            " | Índice: " +
            indiceCoche
        );
    }
    public void Acelador(InputAction.CallbackContext context)
    {
        if (puedoPisarAcelerador)
        {
         acelerador= context.ReadValue<float>();

        }
      
    }
    public void GatilloIzquierdo(InputAction.CallbackContext context)
    {
        Debug.Log(
            "Gatillo izquierdo callback. Phase: " +
            context.phase
        );

        if (context.performed)
        {
            Debug.Log("<<< PALANCA IZQUIERDA DETECTADA >>>");

            ActivarCoche(indiceCoche - 1);
        }
    }

    public void GatilloDerecho(InputAction.CallbackContext context)
    {
        Debug.Log(
            "Gatillo derecho callback. Phase: " +
            context.phase
        );

        if (context.performed)
        {
            Debug.Log(">>> PALANCA DERECHA DETECTADA <<<");

            ActivarCoche(indiceCoche + 1);
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
   /* public void GatilloIzquierdo(InputAction.CallbackContext context)
    {
        Debug.Log(
            "GatilloIzquierdo llamado. Phase: " +
            context.phase
        );

        if (context.performed)
        {
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
            Debug.Log("gatillo izquierdo");
            indiceCoche =indiceCoche-1;
            //if (indiceCoche < 0)
            //{
            //    indiceCoche = listaDeCoches.Count;
            //}
=======
            Debug.Log("PALANCA IZQUIERDA DETECTADA");
>>>>>>> Stashed changes

            ActivarCoche(indiceCoche - 1);
        }
    }
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
   
    public void GatilloDerecho(InputAction.CallbackContext context)
    {
        
        if (context.performed)
        {
            Debug.Log("gatillo derecho");
            indiceCoche =indiceCoche+1;
            //if (indiceCoche>listaDeCoches.Count)
            //{
            //    indiceCoche = 0;
            //}
=======

    public void GatilloDerecho(InputAction.CallbackContext context)
    {
        Debug.Log(
            "GatilloDerecho llamado. Phase: " +
            context.phase
        );

        if (context.performed)
        {
            Debug.Log("PALANCA DERECHA DETECTADA");
>>>>>>> Stashed changes

            ActivarCoche(indiceCoche + 1);
        }
    }
   */
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
    private void OnGatilloIzquierdo(InputAction.CallbackContext context)
    {
        Debug.Log("<<< PALANCA IZQUIERDA DETECTADA >>>");

        ActivarCoche(indiceCoche - 1);
    }

    private void OnGatilloDerecho(InputAction.CallbackContext context)
    {
        Debug.Log(">>> PALANCA DERECHA DETECTADA <<<");

        ActivarCoche(indiceCoche + 1);
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
        if (gameObject.CompareTag("Player"))
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
