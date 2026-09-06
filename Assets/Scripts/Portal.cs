using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Interfaces;

public class Portal : MonoBehaviour, IColision
{
    [SerializeField] GameObject camara;
    [SerializeField] GameObject mundoADesactivar;
    [SerializeField] GameObject mundoACambiar;
    [SerializeField] bool esBrechaFinal;
   

    public void Colision(GameObject jugador, Vector3 direccionColision)
    {
        
        StartCoroutine(CambioDeMundo());
      //  camara.GetComponent<Animator>().SetBool("Blanco",true);
       // SceneManager.LoadScene("Leaderboard");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator CambioDeMundo()
    {
        camara.GetComponent<Animator>().SetBool("Blanco", true);

        yield return new WaitForSeconds(2);
        if (esBrechaFinal )
        {
            SceneManager.LoadScene("Leaderboard");
        }
        else
        {
            mundoADesactivar.SetActive(false);
            mundoACambiar.SetActive(true);
            camara.GetComponent<Animator>().SetBool("Blanco", false);

        }
    }
}
