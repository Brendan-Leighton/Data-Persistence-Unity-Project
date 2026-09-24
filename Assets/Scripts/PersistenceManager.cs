using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistenceManager : MonoBehaviour
{
    // SINGLETON ACCESS POINT
    public static PersistenceManager instance;

    // PERSISTED DATA
    public PlayerScore currPlayer = new(null, 0);
    public PlayerScore previousPlayer = new(null, 0);
    public void SetCurrentPlayer(string name, int score)
    {
        currPlayer.name = name;
        currPlayer.score = score;
    }
    public List<PlayerScore> _leaderboard = new();
    public List<PlayerScore> GetLeaderboard()
    {
        if (_leaderboard == null)
        {
            _leaderboard = new List<PlayerScore>();
        }
        return _leaderboard;
    }

    /// <summary>
    /// The `name` and `score` of a player
    /// </summary>
    [Serializable]
    public class PlayerScore
    {
        public string name;
        public int score;

        public PlayerScore(string p_name, int p_score)
        {
            name = p_name;
            score = p_score;
        }
    }

    /// <summary>
    /// Data structure to save
    /// </summary>
    [Serializable]
    public class SaveData
    {
        public List<PlayerScore> leaderboard = new();
        public PlayerScore previousPlayer = new(null,0);
    }

    /// <summary> "File name to save the data as" </summary>
    private string _saveFileName = "/savefile.json";

    /// <summary>
    /// Setup and manage singleton
    /// </summary>
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (_leaderboard == null)
        {
            _leaderboard = new List<PlayerScore>();
        }
        LoadAll();
        SceneManager.LoadScene("main menu");
    }

    /// <summary>
    /// Save all player's scores
    /// </summary>
    public void Save()
    {
        LogLeaderboard();

        SaveData data = new()
        {
            leaderboard = _leaderboard ?? new List<PlayerScore>(),
            previousPlayer = currPlayer ?? new PlayerScore(null, 0)
        };

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + _saveFileName, json);
    }

    /// <summary>
    /// Load all saved data. Ensures assignment to `_leaderboard` and `currPlayer`
    /// </summary>
    public void LoadAll()
    {
        string path = Application.persistentDataPath + _saveFileName;

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);

            try
            {
                // First try loading as SaveData (with leaderboard)
                SaveData data = JsonUtility.FromJson<SaveData>(json);

                // if we have populated data (data.leaderboard.Count > 0)
                if (data != null && data.leaderboard != null && data.leaderboard.Count > 0)
                {
                    _leaderboard = data.leaderboard;
                    currPlayer = data.previousPlayer ?? new PlayerScore(null, 0);
                    previousPlayer = data.previousPlayer ?? new PlayerScore(null, 0);
                }
                // if we have null data - unpopulated data isn't an issue so we don't need to check it here (leaderboard.Count == 0)
                else if (data == null || data.leaderboard == null)
                {
                    Save();
                    LoadAll();
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to parse save data: {ex.Message}");
                _leaderboard ??= new List<PlayerScore>();
                currPlayer ??= new PlayerScore(null, 0);
            }
        }
        else
        {
            // if null, assign a new value
            _leaderboard ??= new List<PlayerScore>();
            currPlayer ??= new PlayerScore(null, 0);
        }

        LogLeaderboard();
    }

    /// <summary>
    /// Delete saved data for testing purposes.
    /// </summary>
    public void DeleteSavedData()
    {
        LogLeaderboard();

        _leaderboard = new List<PlayerScore>();
        currPlayer = new PlayerScore(null, 0);

        SaveData data = new()
        {
            leaderboard = _leaderboard,
            previousPlayer = currPlayer
        };

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + _saveFileName, json);

        LogLeaderboard();
    }

    /// <summary>
    /// Debug.Log() the data in the leaderboard. Formatted for your pleasure.
    /// </summary>
    public void LogLeaderboard()
    {
        // assign new List if _leaderboard is null
        _leaderboard ??= new List<PlayerScore>();

        //Debug.Log($"Logging Leaderboard...");
        string log = "Leaderboard:";

        if (_leaderboard.Count == 0)
        {
            log += " is empty";
        }
        else
        {
            int position = 1;
            _leaderboard.ForEach(playerScore =>
            {
                if (playerScore != null)
                {
                    log += $"\n\t{position}: {playerScore.name} - {playerScore.score}";
                    position++;
                }
            });
        }

        Debug.Log(log);
    }

    /// <summary>
    /// Updates the persisted leaderboard with the `currPlayer` data if `currPlayer` has a higher score.
    /// </summary>
    public void UpdateLeaderboard()
    {
        Debug.Log("Leaderboard BEFORE update:");
        LogLeaderboard();

        List<PlayerScore> leaderboard = GetLeaderboard();

        leaderboard.Add(currPlayer);
        leaderboard.Sort((a, b) => b.score.CompareTo(a.score));

        PersistenceManager.instance._leaderboard = leaderboard;

        Debug.Log("Leaderboard AFTER update:");
        LogLeaderboard();
    }

    public string StringifyPlayer(PlayerScore player)
    {
        return $"{player.name} - {player.score}";
    }
}
