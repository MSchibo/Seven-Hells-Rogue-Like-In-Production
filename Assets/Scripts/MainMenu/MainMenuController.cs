using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button cancelButton; // Für das Optionsmenü (Cancel Button)
    [SerializeField] private Button cancelButton1; // Für das Create Player Popup (Cancel Button 1)
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject createPlayerPopup; // Popup für Create Player

    [Header("References")]
    [SerializeField] private AudioManager audioManager;

    [Header("Scene Settings")]
    [SerializeField] private string gameSceneName = "GameScene"; // Setze den korrekten Szenennamen im Inspector

    void Start()
    {
        if (audioManager == null)
        {
            audioManager = FindFirstObjectByType<AudioManager>();
            if (audioManager == null)
            {
                Debug.LogError("Kein AudioManager gefunden! Weise einen im Inspector zu. Zeit: " + System.DateTime.Now);
            }
        }

        if (optionsMenu != null)
        {
            optionsMenu.SetActive(false); // Nur das Optionsmenü-Popup deaktivieren
        }
        if (createPlayerPopup != null)
        {
            createPlayerPopup.SetActive(false); // Create Player Popup initial deaktivieren
        }

        // Button-Listener setzen
        if (startButton != null)
        {
            startButton.onClick.RemoveAllListeners();
            startButton.onClick.AddListener(OnStartGame);
            startButton.interactable = true;
        }
        else
        {
            Debug.LogWarning("StartButton ist nicht zugewiesen! Zeit: " + System.DateTime.Now);
        }

        if (optionsButton != null)
        {
            optionsButton.onClick.RemoveAllListeners();
            optionsButton.onClick.AddListener(OnOpenOptions);
            optionsButton.interactable = true; // Sicherstellen, dass der Button klickbar ist
        }
        else
        {
            Debug.LogWarning("OptionsButton ist nicht zugewiesen! Zeit: " + System.DateTime.Now);
        }

        if (quitButton != null)
        {
            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(OnQuitGame);
            quitButton.interactable = true;
        }
        else
        {
            Debug.LogWarning("QuitButton ist nicht zugewiesen! Zeit: " + System.DateTime.Now);
        }

        if (cancelButton != null)
        {
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(OnCancelOptions);
            cancelButton.interactable = true;
        }
        else
        {
            Debug.LogWarning("CancelButton (für Optionsmenü) ist nicht zugewiesen! Zeit: " + System.DateTime.Now);
        }

        if (cancelButton1 != null)
        {
            cancelButton1.onClick.RemoveAllListeners();
            cancelButton1.onClick.AddListener(OnCancelCreatePlayer);
            cancelButton1.interactable = true;
        }
        else
        {
            Debug.LogWarning("CancelButton1 (für Create Player Popup) ist nicht zugewiesen! Zeit: " + System.DateTime.Now);
        }

        // Stelle sicher, dass ein EventSystem existiert
        if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            Debug.LogError("Kein EventSystem in der Szene! Füge ein EventSystem hinzu. Zeit: " + System.DateTime.Now);
        }
    }

    public void OnStartGame()
    {
        if (SceneUtility.GetBuildIndexByScenePath("Assets/Scenes/" + gameSceneName + ".unity") == -1)
        {
            Debug.LogError($"Szene '{gameSceneName}' ist nicht in den Build-Einstellungen oder existiert nicht! Zeit: " + System.DateTime.Now);
            return;
        }

        // ? STOPPE die Musik des MainMenus
        if (audioManager != null)
        {
            audioManager.StopMusic();
            Debug.Log("MainMenu-Musik gestoppt um " + System.DateTime.Now);
        }
        else
        {
            Debug.LogWarning("AudioManager ist null beim StartGame! Zeit: " + System.DateTime.Now);
        }

        Debug.Log("Spiel gestartet um " + System.DateTime.Now);
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnOpenOptions()
    {
        if (optionsMenu != null)
        {
            optionsMenu.SetActive(true);
            Debug.Log("Optionsmenü geöffnet um " + System.DateTime.Now);
        }
        else
        {
            Debug.LogError("OptionsMenu ist nicht zugewiesen! Zeit: " + System.DateTime.Now);
        }
    }

    public void OnQuitGame()
    {
        Debug.Log("Spiel beendet um " + System.DateTime.Now);
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void OnCancelOptions()
    {
        if (optionsMenu != null && optionsMenu.activeSelf)
        {
            optionsMenu.SetActive(false);
            Debug.Log("Optionsmenü geschlossen um " + System.DateTime.Now);
        }
        else
        {
            Debug.LogWarning("Optionsmenü ist nicht aktiv oder nicht zugewiesen! Zeit: " + System.DateTime.Now);
        }
    }

    public void OnCancelCreatePlayer()
    {
        if (createPlayerPopup != null && createPlayerPopup.activeSelf)
        {
            createPlayerPopup.SetActive(false);
            Debug.Log("Create Player Popup geschlossen um " + System.DateTime.Now);
        }
        else
        {
            Debug.LogWarning("Create Player Popup ist nicht aktiv oder nicht zugewiesen! Zeit: " + System.DateTime.Now);
        }
    }

    public void OnOpenCreatePlayer()
    {
        if (createPlayerPopup != null)
        {
            createPlayerPopup.SetActive(true);
            Debug.Log("Create Player Popup geöffnet um " + System.DateTime.Now);
        }
        else
        {
            Debug.LogError("CreatePlayerPopup ist nicht zugewiesen! Zeit: " + System.DateTime.Now);
        }
    }
}


