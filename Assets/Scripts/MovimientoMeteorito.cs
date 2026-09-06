using UnityEngine;
using static Interfaces;

public class MovimientoMeteorito : TipoDeCoche
{
    [SerializeField] Transform zonaAterrizaje;
    [SerializeField] GameObject[] zonasDeFuego;
    GameObject zonaDeFuegoEscogida;
    [SerializeField] GameObject sistemaParticulasExplosion;
    [SerializeField] GameObject craterImpacto;
    [SerializeField] LayerMask mascaraConductor;
    
    LogicaConductor scriptJugador;

    // Start is called before the first frame update
    void Start()
    {
        UbicarDestino();//ubicamos el destino de la rueda meteoro
    }
    private void OnEnable()
    {
        zonasDeFuego = GameObject.FindGameObjectsWithTag("ZonasDeMeteoros");



    }
    private void OnDestroy()
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, 4, mascaraConductor);
        if (colls.Length>0)
        {
            if (colls[0].GetComponent<LogicaConductor>()!=null)//si es conductor
            {
                LogicaConductor scriptConductor = colls[0].GetComponent<LogicaConductor>();
                if (tipoDeCoche==scriptConductor.tipoDeCoche)//si encajo quiere decir que no le afecto
                {

                }
                else//si no encajo si le afecto
                {
                    Debug.Log("impacto");
                    scriptJugador.ReducirRevoluciones();
                    scriptJugador.orientarPorGolpe = true;
                  
                   
                    scriptJugador.puedoGirar = false;
                    scriptJugador.puedoPisarAcelerador = false;
                    scriptJugador.acelerando = false;
                    scriptJugador.acelerador = 1;
                    scriptJugador.multiplicador = 0;
                    scriptJugador.queCocheSoy.GetComponent<ParpadeoGolpe>().IniciarParpadeo();
                    

                    Invoke("PermitirGirarJugador", 0.3F);
                }
               
               
            }
        }
    }
    void PermitirGirarJugador()
    {
        scriptJugador.orientarPorGolpe = false;
        scriptJugador.puedoGirar = true;
        scriptJugador.puedoPisarAcelerador = true;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(zonaAterrizaje);
        Movimiento();
        LlegadaADestino();
    }
    void UbicarDestino()
    {
        zonaDeFuegoEscogida = zonasDeFuego[Random.Range(0, zonasDeFuego.Length)];//escogemos una random entre todas
        float XRandom = Random.Range(zonaDeFuegoEscogida.GetComponent<ZonaDeFuego>().xmin, zonaDeFuegoEscogida.GetComponent<ZonaDeFuego>().xmax);//randomizaremos la ubicacion final del ascuas dentro de los limites de cada zona de fuego
        float YRandom = Random.Range(zonaDeFuegoEscogida.GetComponent<ZonaDeFuego>().ymin, zonaDeFuegoEscogida.GetComponent<ZonaDeFuego>().ymax);

        zonaAterrizaje.transform.position = new Vector3(XRandom, 0, YRandom);
        Vector3 direccionCaida = zonaAterrizaje.position - transform.position;//cogemos la direccion de la caida

        //diana.transform.position = zonaAterrizaje.position;//posicionamos la diana en la zona de aterrizaje
    }

    void LlegadaADestino()
    {
        if (transform.position == zonaAterrizaje.position)
        {
            Instantiate(sistemaParticulasExplosion, transform.position, Quaternion.identity);
            //craterImpacto.SetActive(true);//activamos el crater
            //craterImpacto.transform.SetParent(null);//desemparentamos el crater del padre
            Destroy(transform.parent.gameObject);
        }
    }

    void Movimiento()
    {
        transform.position = Vector3.MoveTowards(transform.position, zonaAterrizaje.position, 30 * Time.deltaTime);
    }
}
