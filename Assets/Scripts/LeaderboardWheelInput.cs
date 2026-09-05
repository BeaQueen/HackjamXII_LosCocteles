using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LeaderboardWheelInput : MonoBehaviour
{
    [Header("Letter selectors")]
    [SerializeField]
    private LetterCarousel[] letterCarousels;

    [Header("Wheel Inputs")]
    [SerializeField]
    private InputActionReference leftButtonAction;

    [SerializeField]
    private InputActionReference rightButtonAction;

    [SerializeField]
    private InputActionReference acceleratorAction;

    [SerializeField]
    private InputActionReference brakeAction;

    [Header("Testing")]
    [SerializeField]
    private float testRaceTime = 42.583f;

    [Header("Scene")]
    [SerializeField]
    private string mainMenuSceneName = "MainMenu";

    private int currentSlot = 0;
    private bool scoreSaved = false;


    private void OnEnable()
    {
        leftButtonAction.action.performed += OnLeftPressed;
        rightButtonAction.action.performed += OnRightPressed;
        acceleratorAction.action.performed += OnAcceleratorPressed;
        brakeAction.action.performed += OnBrakePressed;

        leftButtonAction.action.Enable();
        rightButtonAction.action.Enable();
        acceleratorAction.action.Enable();
        brakeAction.action.Enable();
    }


    private void OnDisable()
    {
        leftButtonAction.action.performed -= OnLeftPressed;
        rightButtonAction.action.performed -= OnRightPressed;
        acceleratorAction.action.performed -= OnAcceleratorPressed;
        brakeAction.action.performed -= OnBrakePressed;

        leftButtonAction.action.Disable();
        rightButtonAction.action.Disable();
        acceleratorAction.action.Disable();
        brakeAction.action.Disable();
    }


    private void OnLeftPressed(
        InputAction.CallbackContext context)
    {
        if (scoreSaved)
            return;

        letterCarousels[currentSlot].PreviousLetter();
    }


    private void OnRightPressed(
        InputAction.CallbackContext context)
    {
        if (scoreSaved)
            return;

        letterCarousels[currentSlot].NextLetter();
    }


    private void OnAcceleratorPressed(
        InputAction.CallbackContext context)
    {
        if (scoreSaved)
            return;

        // Pasamos a la siguiente letra
        if (currentSlot < letterCarousels.Length - 1)
        {
            currentSlot++;
            return;
        }

        // Tercera letra confirmada
        SaveScore();
    }


    private void OnBrakePressed(
        InputAction.CallbackContext context)
    {
        if (scoreSaved)
            return;

        if (currentSlot > 0)
        {
            currentSlot--;
        }
    }


    private string GetPlayerName()
    {
        string playerName = "";

        foreach (LetterCarousel carousel in letterCarousels)
        {
            playerName += carousel.GetCurrentLetter();
        }

        return playerName;
    }


    private void SaveScore()
    {
        if (scoreSaved)
            return;

        scoreSaved = true;

        string playerName = GetPlayerName();

        LeaderboardManager.AddScore(
            playerName,
            testRaceTime
        );

        Debug.Log(
            "SAVED: " +
            playerName +
            " - " +
            testRaceTime
        );

        SceneManager.LoadScene(mainMenuSceneName);
    }
}