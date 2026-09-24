using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    /// <summary>
    /// InputField where player enters their name.
    /// </summary>
    [SerializeField] TMP_Text inputField;

    /// <summary>
    /// InputField where player enters their name.
    /// </summary>
    [SerializeField] TMP_Text lastPlayerPlayedName;

    string currPlayer;
    string prevPlayerName;

    int minNameLength = 3;

    /// <summary>
    /// Load previously saved data and prepopulate the InputField text with the person's name.
    /// </summary>
    void Start()
    {
        prevPlayerName = PersistenceManager.instance.previousPlayer.name;
        if (prevPlayerName.Length >= minNameLength)
        {
            lastPlayerPlayedName.text = $"Welcome Back, {PersistenceManager.instance.previousPlayer.name}!";
        }
        else
        {
            lastPlayerPlayedName.text = $"Welcome, New Player!";
        }
    }

    public void HandleStartGameButton()
    {
        if (inputField.text.Trim().Length < minNameLength + 1)
        {
            if (prevPlayerName.Length < minNameLength)
            {
                Debug.Log("player name not set");
                lastPlayerPlayedName.text = $"You must enter a name with atleast {minNameLength} characters!";
                return;
            }
            else
            {
                PersistenceManager.instance.currPlayer.name = prevPlayerName;
                PersistenceManager.instance.currPlayer.score = 0;
            }
        }
        else
        {
            PersistenceManager.instance.currPlayer.name = inputField.text.Trim();
            PersistenceManager.instance.currPlayer.score = 0;
        }

        StartGame();
    }

    /// <summary>
    /// Save InputField text then load "main" scene
    /// </summary>
    public void StartGame()
    {
        Debug.Log("Starting Game...");
        SceneManager.LoadScene("main");
        Debug.Log("Game should be loaded");
    }

    /// <summary>
    /// Hook up to Reset Data button in UI to delete the saved data.
    /// </summary>
    public void ResetSavedData()
    {
        Debug.Log("Resetting Saved Data...");
        if (PersistenceManager.instance != null)
        {
            PersistenceManager.instance.LogLeaderboard();
            PersistenceManager.instance.DeleteSavedData();
            PersistenceManager.instance.LogLeaderboard();
        }
    }

    /// <summary>
    /// Hook up to UI button to navigate to the Leaderboard scene.
    /// </summary>
    public void OpenLeaderboard()
    {
        Debug.Log("Opening Leaderboard...");

        SceneManager.LoadScene("Leaderboard");
    }
}
