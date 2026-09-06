using UnityEngine;

public class Interfaces : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public interface IColision 
    {
        void Colision(GameObject jugador,Vector3 direccionColision);
    }
    public class TipoDeCoche:MonoBehaviour
    {
        public Coche tipoDeCoche;
        public enum Coche
        {
            Pasado,
            Presente,
            Futuro
        }
    }
}
