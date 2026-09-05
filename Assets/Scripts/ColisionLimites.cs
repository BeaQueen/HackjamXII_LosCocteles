using UnityEngine;
using static Interfaces;

public class ColisionLimites : MonoBehaviour , IColision
{
    LogicaConductor scriptJugador;
    [SerializeField] Transform direccionOrientacion;
    public void Colision(GameObject jugador)
    {
         scriptJugador=jugador.GetComponent<LogicaConductor>();
        scriptJugador.ReducirRevoluciones(4);
        
       scriptJugador.Orientarse(direccionOrientacion.transform.forward,0.1f,jugador.transform);//orientamos segun una referencia que tiene esta colision
       // scriptJugador.Impulso(direccionOrientacion.transform.forward, 20);
        scriptJugador.puedoGirar=false;

        Invoke("PermitirGirarJugador", 0.3F);
    }

    void PermitirGirarJugador()
    {
        scriptJugador.puedoGirar=true;
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
