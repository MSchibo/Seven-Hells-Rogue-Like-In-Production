using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public Toggle musicToggle;
    public Toggle sfxToggle;
    public Button cancelButton; // Neues Feld für Cancel-Button

    [Header("References")]
    public AudioManager audioManager;

    private Resolution[] availableResolutions;

    void Start()
    {
        if (audioManager == null)
        {
            audioManager = FindFirstObjectByType<AudioManager>(); // Ersetze FindObjectOfType
            if (audioManager == null)
            {
                Debug.LogError("Kein AudioManager gefunden! Weise einen im Inspector zu. Zeit: " + System.DateTime.Now);
                return;
            }
        }

        availableResolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        var options = new System.Collections.Generic.List<string>();
        int currentResIndex = 0;

        for (int i = 0; i < availableResolutions.Length; i++)
        {
            string option = availableResolutions[i].width + " x " + availableResolutions[i].height;
            options.Add(option);

            if (availableResolutions[i].width == Screen.currentResolution.width &&
                availableResolutions[i].height == Screen.currentResolution.height)
            {
                currentResIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResIndex;
        resolutionDropdown.RefreshShownValue();

        fullscreenToggle.isOn = Screen.fullScreen;
        musicToggle.isOn = audioManager.IsMusicPlaying();
        sfxToggle.isOn = audioManager.IsSFXEnabled();

        resolutionDropdown.onValueChanged.RemoveAllListeners();
        fullscreenToggle.onValueChanged.RemoveAllListeners();
        musicToggle.onValueChanged.RemoveAllListeners();
        sfxToggle.onValueChanged.RemoveAllListeners();

        resolutionDropdown.onValueChanged.AddListener(SetResolution);
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        musicToggle.onValueChanged.AddListener(OnMusicToggle);
        sfxToggle.onValueChanged.AddListener(OnSFXToggle);

        if (cancelButton != null)
        {
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(OnCancel);
            cancelButton.interactable = true;
        }
        else
        {
            Debug.LogWarning("CancelButton ist nicht zugewiesen! Zeit: " + System.DateTime.Now);
        }
    }

    public void SetResolution(int index)
    {
        Resolution res = availableResolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        Debug.Log("Auflösung geändert: " + res.width + " x " + res.height + " um " + System.DateTime.Now);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        Debug.Log("Vollbildmodus: " + isFullscreen + " um " + System.DateTime.Now);
    }

    public void OnMusicToggle(bool isOn)
    {
        if (audioManager != null)
        {
            audioManager.ToggleMusic(isOn);
        }
        else
        {
            Debug.LogError("AudioManager nicht verfügbar! Zeit: " + System.DateTime.Now);
        }
    }

    public void OnSFXToggle(bool isOn)
    {
        if (audioManager != null)
        {
            audioManager.ToggleSFX(isOn);
            Debug.Log("SFX " + (isOn ? "an" : "aus") + " um " + System.DateTime.Now);
        }
        else
        {
            Debug.LogError("AudioManager nicht verfügbar! Zeit: " + System.DateTime.Now);
        }
    }

    public void OnCancel()
    {
        gameObject.SetActive(false); // Schließe das Optionsmenü
        Debug.Log("Optionsmenü geschlossen um " + System.DateTime.Now);
    }
}

