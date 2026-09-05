using UnityEngine;

public class LeaderboardUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform entriesContainer;
    [SerializeField] private LeaderboardRowUI rowPrefab;

    [Header("Settings")]
    [SerializeField] private int maxEntries = 10;

    private void Start()
    {
        RefreshLeaderboard();
    }

    public void RefreshLeaderboard()
    {
        ClearRows();

        LeaderboardData data = LeaderboardManager.LoadLeaderboard();

        for (int i = 0; i < maxEntries; i++)
        {
            LeaderboardRowUI row =
                Instantiate(rowPrefab, entriesContainer);

            if (i < data.entries.Count)
            {
                LeaderboardEntry entry = data.entries[i];

                row.Setup(
                    i + 1,
                    entry.playerName,
                    entry.time
                );
            }
            else
            {
                row.SetupEmpty(i + 1);
            }
        }
    }

    private void ClearRows()
    {
        foreach (Transform child in entriesContainer)
        {
            Destroy(child.gameObject);
        }
    }
}