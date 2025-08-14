using UnityEngine;
using UnityEngine.UI;

public class AbilityCooldownUI : MonoBehaviour
{
    public Image cooldownOverlay; // halbtransparent, fill method radial
    public Text cooldownText;     // Legacy Text (UnityEngine.UI.Text)
    public float cooldownDuration = 3f;

    private float cooldownTimer;
    private bool isOnCooldown;

    void Start()
    {
        ResetCooldownUI();
    }

    void Update()
    {
        if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;

            if (cooldownTimer <= 0f)
            {
                ResetCooldownUI();
            }
            else
            {
                float fill = cooldownTimer / cooldownDuration;
                cooldownOverlay.fillAmount = fill;
                cooldownText.text = Mathf.CeilToInt(cooldownTimer).ToString("0") + "s";
            }
        }
    }

    public void TriggerCooldown(float duration)
    {
        cooldownDuration = duration;
        cooldownTimer = cooldownDuration;
        isOnCooldown = true;
        cooldownOverlay.gameObject.SetActive(true);
        cooldownOverlay.fillAmount = 1f;
        cooldownText.gameObject.SetActive(true);
    }

    private void ResetCooldownUI()
    {
        isOnCooldown = false;
        cooldownOverlay.fillAmount = 0f;
        cooldownOverlay.gameObject.SetActive(false);
        cooldownText.text = "";
        cooldownText.gameObject.SetActive(false);
    }

    public bool IsReady()
    {
        return !isOnCooldown;
    }
}


