using UnityEngine;
using UnityEngine.UI;

public class BasicAttackCooldownBar : MonoBehaviour
{
    public Image cooldownFill;
    public Text cooldownText;
    public float cooldownDuration = 1f;

    private float currentCooldown = 0f;
    private bool isOnCooldown = false;

    void Update()
    {
        if (isOnCooldown)
        {
            currentCooldown -= Time.deltaTime;
            float fillAmount = Mathf.Clamp01(currentCooldown / cooldownDuration);
            cooldownFill.fillAmount = 1f - fillAmount; // Füllt auf

            if (cooldownText != null)
                cooldownText.text = Mathf.Ceil(currentCooldown).ToString("F0");

            if (currentCooldown <= 0f)
            {
                isOnCooldown = false;
                cooldownFill.fillAmount = 1f;
                cooldownText.text = "";
            }
        }
    }

    public void StartCooldown(float duration)
    {
        cooldownDuration = duration;
        currentCooldown = cooldownDuration;
        isOnCooldown = true;

        if (cooldownText != null)
            cooldownText.text = Mathf.Ceil(cooldownDuration).ToString("F0");
    }

    public bool IsOnCooldown()
    {
        return isOnCooldown;
    }
}

