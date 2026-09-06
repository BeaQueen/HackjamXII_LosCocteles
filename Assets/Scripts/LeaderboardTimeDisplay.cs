using TMPro;
using UnityEngine;

public class LeaderboardTimeDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text timeText;


    private void Start()
    {
        ShowTime();
    }


    private void ShowTime()
    {
        float time =
            RaceResultData.FinalTime;


        int minutes =
            Mathf.FloorToInt(time / 60f);


        int seconds =
            Mathf.FloorToInt(time % 60f);


        int centiseconds =
            Mathf.FloorToInt(
                (time * 100f) % 100f
            );


        timeText.text =
            $"{minutes:00}:{seconds:00}:{centiseconds:00}";
    }
}