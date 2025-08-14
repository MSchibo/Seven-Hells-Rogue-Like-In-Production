using UnityEngine;

public class HitEffect : MonoBehaviour
{
    [SerializeField] private Color hitColor = Color.white;
    [SerializeField] private float hitEffectDuration = 0.1f;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    public void ApplyHitEffect()
    {
        if (spriteRenderer != null)
            StartCoroutine(DoHitEffect());
    }

    private System.Collections.IEnumerator DoHitEffect()
    {
        spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(hitEffectDuration);
        spriteRenderer.color = originalColor;
    }
}

