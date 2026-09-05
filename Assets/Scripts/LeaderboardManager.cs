using System.IO;
using UnityEngine;

public static class LeaderboardManager
{
    private static string SavePath =>
        Path.Combine(Application.persistentDataPath, "leaderboard.json");

    public static void AddScore(string playerName, float time)
    {
        LeaderboardData data = LoadLeaderboard();

        LeaderboardEntry newEntry =
            new LeaderboardEntry(playerName, time);

        data.entries.Add(newEntry);

        // Menor tiempo = mejor posición
        data.entries.Sort((a, b) => a.time.CompareTo(b.time));

        SaveLeaderboard(data);
    }

    public static LeaderboardData LoadLeaderboard()
    {
        if (!File.Exists(SavePath))
        {
            return new LeaderboardData();
        }

        try
        {
            string json = File.ReadAllText(SavePath);

            LeaderboardData data =
                JsonUtility.FromJson<LeaderboardData>(json);

            return data ?? new LeaderboardData();
        }
        catch (System.Exception e)
        {
            Debug.LogError(
                "Could not load leaderboard: " + e.Message
            );

            return new LeaderboardData();
        }
    }

    private static void SaveLeaderboard(LeaderboardData data)
    {
        try
        {
            string json =
                JsonUtility.ToJson(data, true);

            File.WriteAllText(SavePath, json);

            Debug.Log(
                "Leaderboard saved at: " + SavePath
            );
        }
        catch (System.Exception e)
        {
            Debug.LogError(
                "Could not save leaderboard: " + e.Message
            );
        }
    }

    public static void ClearLeaderboard()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
        }

        Debug.Log("Leaderboard deleted.");
    }

    public static string GetSavePath()
    {
        return SavePath;
    }
}