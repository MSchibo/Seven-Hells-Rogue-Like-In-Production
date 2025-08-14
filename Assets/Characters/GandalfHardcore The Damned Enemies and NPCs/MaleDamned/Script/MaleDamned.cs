using UnityEngine;
using TMPro;

public class MaleDamned : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 20f;
    [SerializeField] private float initialHealth = 20f;
    private float currentHealth;

    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 3f;
    [SerializeField] private float knockbackDuration = 0.1f;
    private float knockbackTimer = 0f;
    private Vector2 knockbackDirection;
    private bool isKnockedBack = false;

    [Header("Damage Text")]
    [SerializeField] private GameObject damageTextPrefab;      // Prefab mit TextMeshProUGUI-Komponente
    [SerializeField] private Transform damageTextSpawnPoint;   // Optional, wo der Text erscheinen soll
    [SerializeField] private DamageTextSpawner damageTextSpawner;

    [Header("Animation")]
    private Animator animator;
    private string currentState;
    private bool isFacingEast = true; // Standard-Richtung
    private bool isDying = false;

    [Header("References")]
    private Transform player;
    private HitEffect hitEffect; // Referenz zum HitEffect-Skript

    void Start()
    {
        currentHealth = Mathf.Min(initialHealth, maxHealth);

        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("Animator-Komponente nicht gefunden auf " + gameObject.name);
        }

        hitEffect = GetComponent<HitEffect>();
        if (hitEffect == null)
        {
            Debug.LogWarning("HitEffect-Komponente nicht gefunden auf " + gameObject.name + ". Füge HitEffect hinzu!");
        }

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogWarning("Spieler nicht gefunden! Weise ein GameObject mit Tag 'Player' zu.");
        }

        ChangeAnimationState("Idle East"); // Start mit Idle East
    }

    void Update()
    {
        if (isDying || player == null) return;

        if (isKnockedBack)
        {
            float step = knockbackForce * Time.deltaTime;
            transform.Translate(knockbackDirection * step);

            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0)
            {
                isKnockedBack = false;
                UpdateIdleDirection(); // Rückkehr zur Idle-Animation nach Knockback
            }
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            UpdateIdleDirection(); // Kontinuierliche Anpassung der Idle-Richtung
        }
    }

    /// <summary>
    /// Ändert die Idle-Richtung basierend auf der Spielerposition.
    /// </summary>
    void UpdateIdleDirection()
    {
        if (player != null)
        {
            float directionX = player.position.x - transform.position.x;
            if (directionX > 0 && !isFacingEast)
            {
                ChangeAnimationState("Idle East");
                isFacingEast = true;
            }
            else if (directionX < 0 && isFacingEast)
            {
                ChangeAnimationState("Idle west");
                isFacingEast = false;
            }
        }
    }

    /// <summary>
    /// Enemy erhält Schaden mit Knockback in Richtung vom Angreifer weg.
    /// </summary>
    public void TakeDamage(float damage, Vector3 attackerPosition)
    {
        if (isDying) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"MaleDamned nimmt {damage} Schaden. Verbleibende HP: {currentHealth}");

        if (hitEffect != null)
        {
            hitEffect.ApplyHitEffect();
        }

        knockbackDirection = (transform.position - attackerPosition).normalized;
        isKnockedBack = true;
        knockbackTimer = knockbackDuration;

        ShowDamageText(damage);
    }

    /// <summary>
    /// Overload falls keine Angreifer-Position bekannt ist (z. B. default von vorne)
    /// </summary>
    public void TakeDamage(float damage, Vector3 attackerPosition, bool isCrit = false)
    {
        if (isDying) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        if (hitEffect != null)
            hitEffect.ApplyHitEffect();

        knockbackDirection = (transform.position - attackerPosition).normalized;
        isKnockedBack = true;
        knockbackTimer = knockbackDuration;

        if (damageTextSpawner != null)
            damageTextSpawner.SpawnDamageText(damage, isCrit);
    }

    private void ShowDamageText(float damage)
    {
        if (damageTextPrefab == null)
        {
            Debug.LogWarning("DamageTextPrefab ist nicht gesetzt!");
            return;
        }

        Vector3 spawnPos = damageTextSpawnPoint != null
            ? damageTextSpawnPoint.position
            : transform.position + Vector3.up * 1f;

        GameObject dmgTextObj = Instantiate(damageTextPrefab, spawnPos, Quaternion.identity, transform); // Parent zum MaleDamned
        TextMeshProUGUI tmp = dmgTextObj.GetComponent<TextMeshProUGUI>();
        if (tmp != null)
        {
            tmp.text = "-" + damage.ToString("0");
            Destroy(dmgTextObj, 1f);
        }
        else
        {
            Debug.LogWarning("DamageTextPrefab hat keine TextMeshProUGUI-Komponente!");
            Destroy(dmgTextObj);
        }
    }

    void Die()
    {
        isDying = true;
        ChangeAnimationState(isFacingEast ? "Dying East" : "Dying West");
        Invoke("Respawn", 1f); // Warte 1 Sekunde, bevor Respawn
    }

    void Respawn()
    {
        currentHealth = maxHealth;
        isDying = false;
        UpdateIdleDirection(); // Setze die Richtung nach Respawn
    }

    void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;

        if (animator != null)
        {
            animator.Play(newState, 0);
            currentState = newState;
        }
        else
        {
            Debug.LogError("Animator ist null, kann State " + newState + " nicht abspielen!");
        }
    }
}


