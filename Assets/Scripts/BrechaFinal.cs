using UnityEngine;
using UnityEngine.SceneManagement;

public class BrechaFinal : MonoBehaviour
{
    public void Colision(GameObject jugador, Vector3 direccionColision)
    {
       
        //  camara.GetComponent<Animator>().SetBool("Blanco",true);
         SceneManager.LoadScene("Leaderboard");
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
