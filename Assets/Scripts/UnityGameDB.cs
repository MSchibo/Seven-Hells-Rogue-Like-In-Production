using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;

public class UnityGameDB : MonoBehaviour
{
    [Header("API Settings")]
    public string apiUrl = "http://localhost:5130/api/players";

    [Header("UI References")]
    public GameObject createPlayerPopup;
    public TMP_InputField playerNameInput;
    public TMP_Text playerNameDisplay;
    public TMP_Dropdown playerDropdown;

    private void Start()
    {
        StartCoroutine(LoadPlayersFromApi());
    }

    public void ShowCreatePlayerPopup()
    {
        createPlayerPopup.SetActive(true);
    }

    public void CreatePlayer()
    {
        string playerName = playerNameInput.text;
        if (!string.IsNullOrEmpty(playerName))
        {
            StartCoroutine(SavePlayerToApi(playerName));
        }
        else
        {
            Debug.LogWarning("Spielername ist leer!");
        }
    }

    IEnumerator SavePlayerToApi(string playerName)
    {
        Player playerObj = new Player
        {
            Username = playerName,
            CreatedDate = System.DateTime.Now.ToString("o")
        };

        string jsonData = JsonUtility.ToJson(playerObj);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest www = new UnityWebRequest(apiUrl, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Spieler erfolgreich gespeichert: " + www.downloadHandler.text);
                Player savedPlayer = JsonUtility.FromJson<Player>(www.downloadHandler.text);
                playerNameDisplay.text = "Spieler: " + savedPlayer.Username;
                createPlayerPopup.SetActive(false);
                StartCoroutine(LoadPlayersFromApi());
            }
            else
            {
                Debug.LogError("Fehler beim Speichern: " + www.error + "\n" + www.downloadHandler.text);
            }
        }
    }

    IEnumerator LoadPlayersFromApi()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(apiUrl))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string json = www.downloadHandler.text;
                Debug.Log("API Response: " + json);

                if (string.IsNullOrEmpty(json) || json == "[]")
                {
                    Debug.LogWarning("Keine Spieler in der Datenbank vorhanden.");
                    playerDropdown.ClearOptions();
                    playerNameDisplay.text = "Spieler: Spieler";
                    yield break;
                }

                Player[] players = JsonHelper.FromJson<Player>(json);
                if (players == null || players.Length == 0)
                {
                    Debug.LogWarning("Keine Spieler gefunden nach Deserialisierung.");
                    playerDropdown.ClearOptions();
                    playerNameDisplay.text = "Spieler: Spieler";
                    yield break;
                }

                playerDropdown.ClearOptions();
                var options = new System.Collections.Generic.List<TMP_Dropdown.OptionData>();
                foreach (var player in players)
                {
                    options.Add(new TMP_Dropdown.OptionData(player.Username));
                    Debug.Log("Option hinzugefügt: " + player.Username); // Debugging
                }
                playerDropdown.AddOptions(options);
                if (players.Length > 0)
                {
                    playerNameDisplay.text = "Spieler: " + players[0].Username;
                    playerDropdown.value = 0; // Setze den ersten Eintrag als ausgewählt
                    playerDropdown.RefreshShownValue(); // Aktualisiere die Anzeige
                    Debug.Log("Dropdown-Wert gesetzt auf: " + playerDropdown.options[0].text); // Debugging
                }
            }
            else
            {
                Debug.LogError("Fehler beim Laden der Spieler: " + www.error);
                playerNameDisplay.text = "Spieler: Spieler";
            }
        }
    }

    private void OnDropdownValueChanged(int index)
    {
        if (playerDropdown.options.Count > 0 && index >= 0)
        {
            string selectedPlayer = playerDropdown.options[index].text;
            playerNameDisplay.text = "Spieler: " + selectedPlayer;
            Debug.Log("Ausgewählter Spieler: " + selectedPlayer);
        }
    }
}





