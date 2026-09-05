using UnityEngine;

public static class LeaderboardManager
{
    private const string LeaderboardKey = "Leaderboard";

    public static void AddScore(string playerName, float time)
    {
        LeaderboardData data = LoadLeaderboard();

        LeaderboardEntry newEntry =
            new LeaderboardEntry(playerName, time);

        data.entries.Add(newEntry);

        // En carreras, menos tiempo = mejor posición
        data.entries.Sort((a, b) => a.time.CompareTo(b.time));

        SaveLeaderboard(data);
    }

    public static LeaderboardData LoadLeaderboard()
    {
        if (!PlayerPrefs.HasKey(LeaderboardKey))
        {
            return new LeaderboardData();
        }

        string json = PlayerPrefs.GetString(LeaderboardKey);

        LeaderboardData data =
            JsonUtility.FromJson<LeaderboardData>(json);

        return data ?? new LeaderboardData();
    }

    private static void SaveLeaderboard(LeaderboardData data)
    {
        string json = JsonUtility.ToJson(data);

        PlayerPrefs.SetString(LeaderboardKey, json);
        PlayerPrefs.Save();
    }

    public static void ClearLeaderboard()
    {
        PlayerPrefs.DeleteKey(LeaderboardKey);
        PlayerPrefs.Save();
    }
}