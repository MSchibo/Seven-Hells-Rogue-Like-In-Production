using UnityEngine;
using Unity.Cinemachine;

public class HUDCameraConnector : MonoBehaviour
{
    public Canvas hudCanvas;
    public CinemachineCamera virtualCamera;

    void Start()
    {
        if (virtualCamera != null && hudCanvas != null)
        {
            hudCanvas.worldCamera = virtualCamera.GetComponent<Camera>();
            Debug.Log("HUD manuell mit Kamera verbunden.");
        }
        else
        {
            Debug.LogWarning("HUDCameraConnector: Virtuelle Kamera oder HUD-Leinwand nicht gefunden.");
        }
    }
}

