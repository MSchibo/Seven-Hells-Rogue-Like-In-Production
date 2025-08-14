using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CustomCursor : MonoBehaviour
{
    public static CustomCursor Instance { get; private set; }

    [SerializeField] private Texture2D normalCursorTexture; // Normaler Cursor
    [SerializeField] private Texture2D selectedCursorTexture; // Cursor bei Auswahl
    [SerializeField] private Vector2 hotSpot = Vector2.zero;

    private bool isOverButton = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("CustomCursor-Duplikate erkannt, altes Objekt wird zerstört! Zeit: " + System.DateTime.Now);
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Debug.Log("CustomCursor initialized at " + System.DateTime.Now);

        if (normalCursorTexture == null)
        {
            Debug.LogWarning("Normaler Cursor-Textur nicht zugewiesen! Zeit: " + System.DateTime.Now);
        }
        else if (!IsValidCursorTexture(normalCursorTexture))
        {
            Debug.LogError("Normaler Cursor-Textur ist ungültig! Prüfe die Import-Einstellungen. Zeit: " + System.DateTime.Now);
        }
        else
        {
            Cursor.SetCursor(normalCursorTexture, hotSpot, CursorMode.Auto);
            Debug.Log("Cursor set to normal at " + System.DateTime.Now);
        }

        // Füge ein EventSystem hinzu, falls keines vorhanden ist
        if (FindFirstObjectByType<EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
            Debug.LogWarning("Kein EventSystem gefunden, eines wurde erstellt! Zeit: " + System.DateTime.Now);
        }

        // Hook für Szenenwechsel
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene Loaded: " + scene.name + " - Cursor wird erneut gesetzt. Zeit: " + System.DateTime.Now);
        ForceSetCursor();
    }

    void Update()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = mousePosition
        };

        var raycastResults = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        int uiLayerMask = 1 << LayerMask.NameToLayer("UI");
        var uiResults = raycastResults.FindAll(result => (1 << result.gameObject.layer) == uiLayerMask);

        bool wasOverButton = isOverButton;
        isOverButton = uiResults.Any(result => result.gameObject.GetComponent<Selectable>() != null);

        if (wasOverButton != isOverButton || !Application.isPlaying)
        {
            SetSelectedCursor(isOverButton);
        }
    }

    private void SetSelectedCursor(bool overButton)
    {
        isOverButton = overButton;

        if (selectedCursorTexture != null && !IsValidCursorTexture(selectedCursorTexture))
        {
            Debug.LogError("Auswahl-Cursor-Textur ist ungültig! Prüfe die Import-Einstellungen. Zeit: " + System.DateTime.Now);
            return;
        }

        Texture2D currentTexture = overButton && selectedCursorTexture != null
            ? selectedCursorTexture
            : normalCursorTexture;

        Cursor.SetCursor(currentTexture, hotSpot, CursorMode.Auto);
        string cursorType = overButton && selectedCursorTexture != null ? "Selected" : "Normal";
        Debug.Log($"Cursor gesetzt: {cursorType} at {System.DateTime.Now}");
    }

    private bool IsValidCursorTexture(Texture2D texture)
    {
        return texture != null && texture.isReadable && texture.format == TextureFormat.RGBA32 && texture.mipmapCount <= 1;
    }

    /// <summary>
    /// Ruft den Cursor-Wechsel explizit auf, z. B. nach Szenenwechsel oder Animationen.
    /// </summary>
    public void ForceSetCursor()
    {
        SetSelectedCursor(false);
    }
}

