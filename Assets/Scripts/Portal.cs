using UnityEngine;
using static Interfaces;

public class Portal : MonoBehaviour, IColision
{
    [SerializeField] GameObject camara;
    public void Colision(GameObject jugador)
    {
        camara.GetComponent<Animator>().SetTrigger("Blanco");
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
