using System.Collections;
using UnityEngine;

public class RandomizacionMeteorito : MonoBehaviour
{
    [SerializeField] Transform xMax, xMin, zMax, zMin;
    [SerializeField] GameObject prefabMeteorito;

    [SerializeField] float delayEntreMeteoritos;
    Coroutine llamadaCorrutina;
    [SerializeField] bool activar;
    [SerializeField] int cantidadDeMeteoritos;
   

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(RutinaInstanciaMeteoros());
    }

    // Update is called once per frame
    void Update()
    {
      
    }
    IEnumerator RutinaInstanciaMeteoros()
    {

        while (true)
        {
            if (cantidadDeMeteoritos == 0)
            {
                yield return new WaitForEndOfFrame();
            }
            else
            {
                for (int i = 0; i < cantidadDeMeteoritos; i++)
                {
                    Vector3 posRandom = new Vector3(Random.Range(xMin.transform.position.x, xMax.transform.position.x), transform.position.y, Random.Range(zMin.transform.position.z, zMax.transform.position.z));
                    Instantiate(prefabMeteorito, posRandom, Quaternion.identity);//instanciamos en una posicion aleatoria al meteoro
                    yield return new WaitForSeconds(Random.Range(0.5f, 3));//delay entre las que se instancia en una misma tanda

                }
                yield return new WaitForSeconds(delayEntreMeteoritos);//damos un margen entre las tandas de instancias

            }



        }



    }
   


    public void ActivarEvento(GameObject managerEntorno)
    {

        // StartCoroutine(RutinaInstanciaMeteoros());
        if (activar)
        {
            llamadaCorrutina = StartCoroutine(RutinaInstanciaMeteoros());
            activar = false;
        }
    }
}
