using UnityEngine;
using TMPro;

public class DamageTextSpawner : MonoBehaviour
{
    [SerializeField] private GameObject damageTextPrefab; // Das World-Space-Canvas-Text-Prefab
    [SerializeField] private Transform spawnOffsetPoint;  // Optional, sonst über dem Kopf
    [SerializeField] private Vector3 offset = new Vector3(0, 1f, 0);

    public void SpawnDamageText(float damageAmount, bool isCrit)
    {
        if (damageTextPrefab == null) return;

        Vector3 spawnPos = spawnOffsetPoint != null ? spawnOffsetPoint.position + offset : transform.position + offset;
        GameObject textObj = Instantiate(damageTextPrefab, spawnPos, Quaternion.identity);

        TextMeshProUGUI tmp = textObj.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null)
        {
            tmp.text = "-" + damageAmount.ToString("0");

            if (isCrit)
            {
                tmp.fontSize = 44;
                tmp.color = Color.yellow;
                tmp.fontMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth, 0.3f);
                tmp.fontMaterial.SetColor(ShaderUtilities.ID_OutlineColor, Color.red);
            }
            else
            {
                tmp.fontSize = 32;
                tmp.color = Color.yellow;
                tmp.fontMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth, 0f);
            }
        }
    }
}
