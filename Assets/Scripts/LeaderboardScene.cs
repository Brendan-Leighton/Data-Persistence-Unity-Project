using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LeaderboardScene : MonoBehaviour
{
    [SerializeField] public List<TMP_Text> leaderboardNames;
    [SerializeField] public List<TMP_Text> leaderboardScores;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PersistenceManager.instance == null)
        {
            Debug.LogError("PersistenceManager.instance is null");
            return;
        }

        List<PersistenceManager.PlayerScore> leaderboard = PersistenceManager.instance.GetLeaderboard();
        if (leaderboard == null || leaderboardNames == null || leaderboardScores == null)
        {
            Debug.LogWarning("leaderboardTextElements is null. Did you forget to assign them in the Hierarchy?");
            return;
        }

        // only showing the top 5 scores
        for (int i = 0; i < 5; i++)
        {
            bool hasRecord = i < leaderboard.Count && leaderboard[i] != null;

            leaderboardNames[i].text = hasRecord ? leaderboard[i].name : "...";
            leaderboardScores[i].text = hasRecord ? leaderboard[i].score.ToString() : "...";
        }
    }

    public void ToMainMenu()
    {
        SceneManager.LoadScene("main menu");
    }
}
