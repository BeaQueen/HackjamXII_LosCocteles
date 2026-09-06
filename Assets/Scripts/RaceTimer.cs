using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaceTimer : MonoBehaviour
{
    // =========================================================
    // TIMER
    // =========================================================

    [Header("Timer")]

    [Tooltip("Duración máxima de la carrera en segundos")]
    [SerializeField] private float raceDuration = 180f;

    [Tooltip("Texto del cronómetro en el Canvas")]
    [SerializeField] private TMP_Text timerText;


    // =========================================================
    // START DELAY
    // =========================================================

    [Header("Start Delay")]

    [Tooltip("Tiempo que espera antes de empezar la cuenta atrás")]
    [SerializeField] private float startDelay = 3f;


    // =========================================================
    // SCENES
    // =========================================================

    [Header("Scenes")]

    [SerializeField]
    private string leaderboardSceneName =
        "LeaderboardEntry";

    [SerializeField]
    private string mainMenuSceneName =
        "MainMenu";


    // =========================================================
    // GAME OVER
    // =========================================================

    [Header("Game Over")]

    [Tooltip("Efecto glitch + blackout de la cámara")]
    [SerializeField] private SpaceTimeCatchFX blackoutEffect;

    [Tooltip("Tiempo que permanece en negro antes de volver al menú")]
    [SerializeField] private float gameOverDelay = 3f;


    // =========================================================
    // INTERNAL
    // =========================================================

    private float remainingTime;

    private bool timerRunning = false;
    private bool raceEnded = false;


    // =========================================================
    // UNITY
    // =========================================================

    private void Start()
    {
        remainingTime = raceDuration;

        RaceResultData.Clear();

        UpdateTimerText();

        StartCoroutine(
            StartRaceDelay()
        );
    }


    private void Update()
    {
        if (!timerRunning)
            return;

        if (raceEnded)
            return;


        remainingTime -= Time.deltaTime;


        if (remainingTime <= 0f)
        {
            remainingTime = 0f;

            UpdateTimerText();

            LoseRace();

            return;
        }


        UpdateTimerText();
    }


    // =========================================================
    // START DELAY
    // =========================================================

    private IEnumerator StartRaceDelay()
    {
        timerRunning = false;

        Debug.Log(
            "La carrera comienza en "
            + startDelay
            + " segundos."
        );


        // Tiempo real para que no dependa de Time.timeScale
        yield return new WaitForSecondsRealtime(
            startDelay
        );


        if (raceEnded)
            yield break;


        timerRunning = true;

        Debug.Log(
            "¡CRONÓMETRO INICIADO!"
        );
    }


    // =========================================================
    // TIMER TEXT
    // =========================================================

    private void UpdateTimerText()
    {
        if (timerText == null)
            return;


        int minutes =
            Mathf.FloorToInt(
                remainingTime / 60f
            );


        int seconds =
            Mathf.FloorToInt(
                remainingTime % 60f
            );


        int centiseconds =
            Mathf.FloorToInt(
                (remainingTime * 100f) % 100f
            );


        timerText.text =
            $"{minutes:00}:{seconds:00}:{centiseconds:00}";
    }


    // =========================================================
    // WIN
    // =========================================================

    public void CompleteRace()
    {
        if (raceEnded)
            return;


        raceEnded = true;
        timerRunning = false;


        float completionTime =
            raceDuration -
            remainingTime;


        RaceResultData.SetFinalTime(
            completionTime
        );


        Debug.Log(
            "CARRERA COMPLETADA | Tiempo: "
            + completionTime
        );


        Time.timeScale = 1f;


        SceneManager.LoadScene(
            leaderboardSceneName
        );
    }


    // =========================================================
    // LOSE
    // =========================================================

    private void LoseRace()
    {
        if (raceEnded)
            return;


        raceEnded = true;
        timerRunning = false;


        Debug.Log(
            "TIEMPO AGOTADO - GAME OVER"
        );


        StartCoroutine(
            LoseRaceSequence()
        );
    }


    private IEnumerator LoseRaceSequence()
    {
        // Congelamos el gameplay
        Time.timeScale = 0f;


        // Activamos glitch + blackout
        if (blackoutEffect != null)
        {
            blackoutEffect.PlayEffect();
        }


        // Esperamos 3 segundos reales
        yield return new WaitForSecondsRealtime(
            gameOverDelay
        );


        Time.timeScale = 1f;


        SceneManager.LoadScene(
            mainMenuSceneName
        );
    }
}