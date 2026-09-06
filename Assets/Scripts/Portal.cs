using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Interfaces;

public class Portal : MonoBehaviour, IColision
{
    // =========================================================
    // PORTAL
    // =========================================================

    [Header("Portal")]

    [SerializeField]
    private GameObject camara;

    [SerializeField]
    private GameObject mundoADesactivar;

    [SerializeField]
    private GameObject mundoACambiar;

    [SerializeField]
    private bool esBrechaFinal;


    // =========================================================
    // RACE
    // =========================================================

    [Header("Race")]

    [Tooltip(
        "RaceTimer de la escena. Solo es necesario para la brecha final."
    )]
    [SerializeField]
    private RaceTimer raceTimer;


    // =========================================================
    // INTERNAL
    // =========================================================

    private bool cambioEnProceso = false;


    // =========================================================
    // COLLISION
    // =========================================================

    public void Colision(
        GameObject jugador,
        Vector3 direccionColision)
    {
        // Evitamos ejecutar varias veces el portal
        // mientras el jugador sigue tocándolo.
        if (cambioEnProceso)
            return;


        cambioEnProceso = true;


        // Si es la meta final,
        // guardamos el tiempo JUSTO EN ESTE MOMENTO.
        //
        // Los 2 segundos de transición visual
        // NO cuentan para el leaderboard.

        if (esBrechaFinal)
        {
            if (raceTimer != null)
            {
                raceTimer.RegisterRaceFinish();

                Debug.Log(
                    "PORTAL FINAL: tiempo de carrera registrado."
                );
            }
            else
            {
                Debug.LogError(
                    "PORTAL FINAL: RaceTimer no está asignado."
                );
            }
        }


        StartCoroutine(
            CambioDeMundo()
        );
    }


    // =========================================================
    // WORLD CHANGE
    // =========================================================

    private IEnumerator CambioDeMundo()
    {
        // Activamos transición blanca
        if (camara != null)
        {
            Animator animator =
                camara.GetComponent<Animator>();

            if (animator != null)
            {
                animator.SetBool(
                    "Blanco",
                    true
                );
            }
        }


        // Esperamos a que termine la transición
        yield return new WaitForSeconds(2f);


        // =====================================================
        // FINAL PORTAL
        // =====================================================

        if (esBrechaFinal)
        {
            Debug.Log(
                "Cargando Leaderboard. Tiempo guardado: "
                + RaceResultData.FinalTime
            );


            SceneManager.LoadScene(
                "Leaderboard"
            );
        }


        // =====================================================
        // NORMAL PORTAL
        // =====================================================

        else
        {
            if (mundoADesactivar != null)
            {
                mundoADesactivar.SetActive(
                    false
                );
            }


            if (mundoACambiar != null)
            {
                mundoACambiar.SetActive(
                    true
                );
            }


            if (camara != null)
            {
                Animator animator =
                    camara.GetComponent<Animator>();

                if (animator != null)
                {
                    animator.SetBool(
                        "Blanco",
                        false
                    );
                }
            }


            cambioEnProceso = false;
        }
    }
}