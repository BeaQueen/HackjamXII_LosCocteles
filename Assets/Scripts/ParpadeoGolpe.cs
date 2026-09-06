using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ParpadeoGolpe : MonoBehaviour
{
    [SerializeField] List<Renderer> renderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator Parpadeo()
    {
        for (int i = 0; i < renderer.Count; i++)
        {
            renderer[i].enabled = false;//desactivamos el renderer por un tiempo
        }
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < renderer.Count; i++)
        {
            renderer[i].enabled = true;//desactivamos el renderer por un tiempo
        }
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < renderer.Count; i++)
        {
            renderer[i].enabled = false;//desactivamos el renderer por un tiempo
        }
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < renderer.Count; i++)
        {
            renderer[i].enabled = true;//desactivamos el renderer por un tiempo
        }
        for (int i = 0; i < renderer.Count; i++)
        {
            renderer[i].enabled = false;//desactivamos el renderer por un tiempo
        }
    }
}
