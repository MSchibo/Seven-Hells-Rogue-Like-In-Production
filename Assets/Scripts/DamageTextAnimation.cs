using UnityEngine;
using TMPro;

public class DamageTextAnimation : MonoBehaviour
{
    public float floatSpeed = 40f;
    public float lifetime = 1.2f;
    public float fadeDuration = 0.5f;
    public float startScale = 1.2f;

    private TextMeshProUGUI tmp;
    private RectTransform rect;
    private Color originalColor;
    private float timer;

    void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        rect = GetComponent<RectTransform>();

        if (tmp != null)
            originalColor = tmp.color;

        transform.localScale = Vector3.one * startScale;
    }

    void Update()
    {
        timer += Time.deltaTime;

        rect.anchoredPosition += Vector2.up * floatSpeed * Time.deltaTime;
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, Time.deltaTime * 5f);

        if (timer > lifetime - fadeDuration && tmp != null)
        {
            float t = 1 - ((timer - (lifetime - fadeDuration)) / fadeDuration);
            tmp.color = new Color(originalColor.r, originalColor.g, originalColor.b, t);
        }

        if (timer >= lifetime)
            Destroy(gameObject);
    }
}

