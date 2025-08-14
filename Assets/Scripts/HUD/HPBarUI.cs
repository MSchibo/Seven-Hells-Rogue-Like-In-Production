using UnityEngine;
using UnityEngine.UI;

public class HPBarUI : MonoBehaviour
{
    public Image hpFillImage;   // HPBarFill
    public Text hpText;         // HPText

    private int currentHP;
    private int maxHP;

    public void UpdateHP(int current, int max)
    {
        currentHP = current;
        maxHP = max;

        float fillAmount = (float)current / max;
        hpFillImage.fillAmount = fillAmount;
        hpText.text = $"{current} / {max} HP";
    }
}
