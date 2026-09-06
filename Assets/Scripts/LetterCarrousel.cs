using TMPro;
using UnityEngine;

public class LetterCarousel : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TMP_Text previousLetterText;
    [SerializeField] private TMP_Text currentLetterText;
    [SerializeField] private TMP_Text nextLetterText;

    [Header("Selection Feedback")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.yellow;

    private const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    private int currentLetterIndex = 0;

    private void Start()
    {
        UpdateLetters();
    }

    public void NextLetter()
    {
        currentLetterIndex++;

        if (currentLetterIndex >= alphabet.Length)
            currentLetterIndex = 0;

        UpdateLetters();
    }

    public void PreviousLetter()
    {
        currentLetterIndex--;

        if (currentLetterIndex < 0)
            currentLetterIndex = alphabet.Length - 1;

        UpdateLetters();
    }

    private void UpdateLetters()
    {
        int previousIndex = currentLetterIndex - 1;
        int nextIndex = currentLetterIndex + 1;

        if (previousIndex < 0)
            previousIndex = alphabet.Length - 1;

        if (nextIndex >= alphabet.Length)
            nextIndex = 0;

        previousLetterText.text =
            alphabet[previousIndex].ToString();

        currentLetterText.text =
            alphabet[currentLetterIndex].ToString();

        nextLetterText.text =
            alphabet[nextIndex].ToString();
    }

    public char GetCurrentLetter()
    {
        return alphabet[currentLetterIndex];
    }

    public void SetSelected(bool selected)
    {
        if (selected)
        {
            currentLetterText.color = selectedColor;
        }
        else
        {
            currentLetterText.color = normalColor;
        }
    }
}