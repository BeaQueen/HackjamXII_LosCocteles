using TMPro;
using UnityEngine;

public class LeaderboardRowUI : MonoBehaviour
{
    [SerializeField] private TMP_Text positionText;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text timeText;

    public void Setup(int position, string playerName, float time)
    {
        positionText.text = position + ".";
        nameText.text = playerName;
        timeText.text = FormatTime(time);
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 1000f) % 1000f);

        return $"{minutes:00}:{seconds:00}.{milliseconds:000}";
    }
    public void SetupEmpty(int position)
    {
        positionText.text = position + ".";
        nameText.text = "---";
        timeText.text = "--:--.---";
    }
}