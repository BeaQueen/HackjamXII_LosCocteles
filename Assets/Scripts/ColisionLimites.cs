using UnityEngine;
using static Interfaces;

public class ColisionLimites : TipoDeCoche, IColision
{
    LogicaConductor scriptJugador;
    [SerializeField] Transform direccionOrientacion;
    
   

    public void Colision(GameObject jugador, Vector3 direccionColision)
    {
       
        scriptJugador = jugador.GetComponent<LogicaConductor>();
        if (tipoDeCoche==scriptJugador.tipoDeCoche )//si soy el mismo significa que no le afecto
        {
            GetComponent<Collider>().isTrigger = true;

        }
        else
        {
            GetComponent<Collider>().isTrigger = false;
            scriptJugador.ReducirRevoluciones();
            scriptJugador.orientarPorGolpe = true;
            //scriptJugador.Orientarse(direccionColision, 0.1f, jugador.transform);//orientamos segun una referencia que tiene esta colision
            if (direccionOrientacion != null)
            {
               scriptJugador.direccionOrientacionPorGolpe = direccionColision+direccionOrientacion.forward;                                                                                     // scriptJugador.Impulso(direccionOrientacion.transform.forward, 20);

            }
            scriptJugador.puedoGirar = false;
            scriptJugador.puedoPisarAcelerador = false;
            scriptJugador.acelerando = false;
            scriptJugador.acelerador = 1;
            scriptJugador.multiplicador = 0;
            scriptJugador.queCocheSoy.GetComponent<ParpadeoGolpe>().IniciarParpadeo();

            Invoke("PermitirGirarJugador", 0.3F);

        }
       
    }

    void PermitirGirarJugador()
    {
        scriptJugador.orientarPorGolpe=false;
        scriptJugador.puedoGirar=true;
        scriptJugador.puedoPisarAcelerador=true;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
