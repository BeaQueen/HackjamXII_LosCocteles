using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PolvoCoche : MonoBehaviour
{
    [SerializeField] List<ParticleSystem> particulasPolvo;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ActivarParticulasPolvo()
    {
        for (int i = 0; i < particulasPolvo.Count; i++)
        {
            
            particulasPolvo[i].Play(true);
            //if (particulasPolvo[i].GetComponent<Animator>() != null)
            //{
            //    particulasPolvo[i].GetComponent<Animator>().SetBool("Estoy", false);
            //}
            //particulasPolvo[i].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            //particulasPolvo[i].Clear(true);
            //particulasPolvo[i].Play(true);
        }
    }
    public void DesactivarParticulasPolvo()
    {
        for (int i = 0; i < particulasPolvo.Count; i++)
        {

            // particulasPolvo[i].Stop(true, ParticleSystemStopBehavior.StopEmitting);
             particulasPolvo[i].Pause(true);
            //  particulasPolvo[i].Stop(true);
            //if (particulasPolvo[i].GetComponent<Animator>() != null)
            //{
            //    particulasPolvo[i].GetComponent<Animator>().SetBool("Estoy", true);
            //}
        }
    }
}
