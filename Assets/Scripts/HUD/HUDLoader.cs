using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

public class HUDLoader : MonoBehaviour
{
    public static HUDLoader Instance;

    [SerializeField] private string hudSceneName = "HUDScene";
    private bool hudLoaded = false;

    public bool IsLoaded => hudLoaded; // Public-Eigenschaft für den Zugriff

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Debug.LogWarning("HUDLoader: Eine weitere Instanz wurde zerstört.");
            Destroy(gameObject);
            return;
        }
    }

    public void LoadHUDOnce()
    {
        if (hudLoaded) return;

        SceneManager.LoadSceneAsync(hudSceneName, LoadSceneMode.Additive).completed += (op) =>
        {
            Scene hudScene = SceneManager.GetSceneByName(hudSceneName);
            if (hudScene.IsValid())
            {
                Debug.Log("HUDScene geladen, prüfe Root-Objekte...");
                foreach (GameObject rootObj in hudScene.GetRootGameObjects())
                {
                    Debug.Log("Gefundenes Root-Objekt: " + rootObj.name);
                    if (rootObj.name == "HUDRoot")
                    {
                        DontDestroyOnLoad(rootObj);
                        rootObj.SetActive(true);
                        Debug.Log("HUDRoot gefunden und aktiviert.");

                        Canvas hudCanvas = rootObj.GetComponentInChildren<Canvas>();
                        if (hudCanvas != null)
                        {
                            CinemachineCamera virtualCam = FindFirstObjectByType<CinemachineCamera>();
                            if (virtualCam != null)
                            {
                                hudCanvas.worldCamera = virtualCam.GetComponent<Camera>();
                                Debug.Log("HUDCanvas mit Kamera verbunden.");
                            }
                            else
                            {
                                Debug.LogWarning("Keine CinemachineCamera gefunden.");
                            }

                            // Deaktiviere Fähigkeitsslots (Q, E, R) initial
                            DisableAbilitySlots(rootObj);
                        }
                        else
                        {
                            Debug.LogWarning("Keine HUD-Leinwand gefunden.");
                        }
                    }
                }
            }
            else
            {
                Debug.LogWarning("HUDScene konnte nicht validiert werden.");
            }
            hudLoaded = true;
            Debug.Log("HUD geladen: " + hudLoaded);
        };
    }

    public void UnloadHUD()
    {
        if (!hudLoaded) return;

        Scene hudScene = SceneManager.GetSceneByName(hudSceneName);
        if (hudScene.IsValid())
        {
            foreach (GameObject rootObj in hudScene.GetRootGameObjects())
            {
                if (rootObj.name == "HUDRoot")
                {
                    Destroy(rootObj);
                }
            }
        }
        hudLoaded = false;
    }

    private void DisableAbilitySlots(GameObject hudRoot)
    {
        // Suche nach UI-Elementen für Q, E, R (angenommene Namen)
        GameObject qSlot = hudRoot.transform.Find("QSlot")?.gameObject;
        GameObject eSlot = hudRoot.transform.Find("ESlot")?.gameObject;
        GameObject rSlot = hudRoot.transform.Find("RSlot")?.gameObject;

        if (qSlot != null) qSlot.SetActive(false);
        if (eSlot != null) eSlot.SetActive(false);
        if (rSlot != null) rSlot.SetActive(false);

        Debug.Log("Fähigkeitsslots (Q, E, R) initial deaktiviert.");
    }

    // Methode, um Slots später freizuschalten (wird vom SoulSystem aufgerufen)
    public void EnableAbilitySlot(string slotName)
    {
        if (!hudLoaded) return;

        Scene hudScene = SceneManager.GetSceneByName(hudSceneName);
        if (hudScene.IsValid())
        {
            foreach (GameObject rootObj in hudScene.GetRootGameObjects())
            {
                if (rootObj.name == "HUDRoot")
                {
                    GameObject slot = rootObj.transform.Find(slotName + "Slot")?.gameObject;
                    if (slot != null)
                    {
                        slot.SetActive(true);
                        Debug.Log($"Fähigkeitsslot {slotName} freigeschaltet.");
                    }
                }
            }
        }
    }
}
