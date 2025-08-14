using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProfileManager : MonoBehaviour
{
    [SerializeField] private TMP_Text currentPlayerNameText;
    [SerializeField] private TMP_Dropdown playerDropdown;
    [SerializeField] private Button deletePlayerButton;

    private List<string> playerNames = new List<string>();
    private string currentPlayerName = "";

    private const string PLAYER_PREFS_KEY = "PlayerNames";
    private const string CURRENT_PLAYER_KEY = "CurrentPlayer";

    void Start()
    {
        LoadPlayers();
        SetupDropdown();
        UpdateCurrentPlayerDisplay();

        playerDropdown.onValueChanged.AddListener(OnPlayerSelected);
        deletePlayerButton.onClick.AddListener(DeleteCurrentPlayer);
    }

    void LoadPlayers()
    {
        // Lade gespeicherte Spielernamen aus PlayerPrefs
        var savedNames = PlayerPrefs.GetString(PLAYER_PREFS_KEY, "");
        if (!string.IsNullOrEmpty(savedNames))
            playerNames = new List<string>(savedNames.Split('|'));

        currentPlayerName = PlayerPrefs.GetString(CURRENT_PLAYER_KEY, "");
    }

    void SavePlayers()
    {
        PlayerPrefs.SetString(PLAYER_PREFS_KEY, string.Join("|", playerNames));
        PlayerPrefs.SetString(CURRENT_PLAYER_KEY, currentPlayerName);
        PlayerPrefs.Save();
    }

    void SetupDropdown()
    {
        playerDropdown.ClearOptions();
        playerDropdown.AddOptions(playerNames);

        int index = playerNames.IndexOf(currentPlayerName);
        if (index >= 0)
            playerDropdown.value = index;
    }

    void OnPlayerSelected(int index)
    {
        if (index >= 0 && index < playerNames.Count)
        {
            currentPlayerName = playerNames[index];
            UpdateCurrentPlayerDisplay();
            SavePlayers();
        }
    }

    void DeleteCurrentPlayer()
    {
        if (string.IsNullOrEmpty(currentPlayerName))
            return;

        playerNames.Remove(currentPlayerName);
        currentPlayerName = playerNames.Count > 0 ? playerNames[0] : "";
        SavePlayers();
        SetupDropdown();
        UpdateCurrentPlayerDisplay();
    }

    void UpdateCurrentPlayerDisplay()
    {
        currentPlayerNameText.text =
            string.IsNullOrEmpty(currentPlayerName)
                ? "Kein Spieler ausgewählt"
                : $"Aktueller Spieler: {currentPlayerName}";
    }

    public void CreateNewPlayer(string newName)
    {
        if (!playerNames.Contains(newName))
        {
            playerNames.Add(newName);
            currentPlayerName = newName;
            SavePlayers();
            SetupDropdown();
            UpdateCurrentPlayerDisplay();
        }
    }
}

