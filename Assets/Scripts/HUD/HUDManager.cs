using UnityEngine;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }

    [Header("HUD UI Prefabs")]
    public GameObject hpBarUIPrefab;
    public GameObject attackCooldownBarPrefab;
    public GameObject dodgeUIPrefab;
    public GameObject abilityQUIPrefab;
    public GameObject abilityEUIPrefab;
    public GameObject abilityRUIPrefab;

    private HPBarUI hpBarUI;
    private BasicAttackCooldownBar attackCooldownBar;
    private AbilityCooldownUI dodgeUI;
    private AbilityCooldownUI abilityQUI;
    private AbilityCooldownUI abilityEUI;
    private AbilityCooldownUI abilityRUI;

    private bool isInitialized = false; // Flag, um doppelte Initialisierung zu verhindern

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // DontDestroyOnLoad(gameObject); // Aktiviere nur, wenn nötig und als Root-Objekt
        Debug.Log("HUDManager Awake aufgerufen, HUDRoot: " + gameObject.name + ", Ist Root: " + (transform.parent == null));
        if (!isInitialized)
        {
            InitializeHUD();
        }
        else
        {
            Debug.LogWarning("HUDManager wurde bereits initialisiert, überspringe InitializeHUD.");
        }
    }

    void InitializeHUD()
    {
        Debug.Log("Initialisiere HUD, Suche nach Canvas...");
        Transform canvas = transform.Find("Canvas");
        if (canvas == null)
        {
            Debug.LogWarning("Canvas nicht gefunden unter " + transform.name + "! Kinder: " + ListChildren(transform));
            return;
        }
        Debug.Log("Canvas gefunden: " + canvas.name);

        Debug.Log("Suche nach HuDPanel unter Canvas...");
        Transform hudPanel = canvas.Find("HuDPanel");
        if (hudPanel == null)
        {
            Debug.LogWarning("HuDPanel nicht gefunden unter " + canvas.name + "! Kinder: " + ListChildren(canvas));
            return;
        }
        Debug.Log("HuDPanel gefunden: " + hudPanel.name);

        Transform skillBar = hudPanel.Find("SkillBar");
        if (skillBar == null)
        {
            Debug.LogWarning("SkillBar nicht gefunden unter " + hudPanel.name + "! Kinder: " + ListChildren(hudPanel));
            return;
        }
        Debug.Log("SkillBar gefunden: " + skillBar.name + ", Anzahl Kinder vor Instanziierung: " + skillBar.childCount);

        // HP-Bar und AttackCooldown unter HuDPanel
        if (hpBarUIPrefab != null && hpBarUI == null)
        {
            GameObject hpBarInstance = Instantiate(hpBarUIPrefab, hudPanel);
            hpBarUI = hpBarInstance.GetComponent<HPBarUI>();
            Debug.Log("HP-Bar instanziert: " + (hpBarUI != null));
        }
        if (attackCooldownBarPrefab != null && attackCooldownBar == null)
        {
            GameObject cooldownBarInstance = Instantiate(attackCooldownBarPrefab, hudPanel);
            attackCooldownBar = cooldownBarInstance.GetComponent<BasicAttackCooldownBar>();
            Debug.Log("AttackCooldownBar instanziert: " + (attackCooldownBar != null));
        }

        // Fähigkeiten unter SkillBar
        if (dodgeUIPrefab != null && dodgeUI == null)
        {
            GameObject dodgeInstance = Instantiate(dodgeUIPrefab, skillBar);
            dodgeUI = dodgeInstance.GetComponent<AbilityCooldownUI>();
            Debug.Log("DodgeUI instanziert: " + (dodgeUI != null));
        }
        if (abilityQUIPrefab != null && abilityQUI == null)
        {
            GameObject qInstance = Instantiate(abilityQUIPrefab, skillBar);
            abilityQUI = qInstance.GetComponent<AbilityCooldownUI>();
            Debug.Log("AbilityQ UI instanziert: " + (abilityQUI != null));
        }
        if (abilityEUIPrefab != null && abilityEUI == null)
        {
            GameObject eInstance = Instantiate(abilityEUIPrefab, skillBar);
            abilityEUI = eInstance.GetComponent<AbilityCooldownUI>();
            Debug.Log("AbilityE UI instanziert: " + (abilityEUI != null));
        }
        if (abilityRUIPrefab != null && abilityRUI == null)
        {
            GameObject rInstance = Instantiate(abilityRUIPrefab, skillBar);
            abilityRUI = rInstance.GetComponent<AbilityCooldownUI>();
            Debug.Log("AbilityR UI instanziert: " + (abilityRUI != null));
        }

        Debug.Log("SkillBar nach Instanziierung, Anzahl Kinder: " + skillBar.childCount);
        isInitialized = true;
        Debug.Log("HUD initialisiert: hpBarUI=" + (hpBarUI != null) + ", attackCooldownBar=" + (attackCooldownBar != null) +
                  ", dodgeUI=" + (dodgeUI != null) + ", abilityQUI=" + (abilityQUI != null) +
                  ", abilityEUI=" + (abilityEUI != null) + ", abilityRUI=" + (abilityRUI != null));
    }

    private string ListChildren(Transform parent)
    {
        string children = "";
        foreach (Transform child in parent)
        {
            children += child.name + ", ";
        }
        return children.Length > 0 ? children.TrimEnd(',', ' ') : "Keine Kinder";
    }

    public void UpdateHP(int currentHP, int maxHP)
    {
        if (hpBarUI != null) hpBarUI.UpdateHP(currentHP, maxHP);
    }

    public void StartAttackCooldown(float cooldown)
    {
        if (attackCooldownBar != null) attackCooldownBar.StartCooldown(cooldown);
    }

    public void TriggerDodgeCooldown(float cooldown)
    {
        if (dodgeUI != null) dodgeUI.TriggerCooldown(cooldown);
    }

    public void TriggerAbilityQCooldown(float cooldown)
    {
        if (abilityQUI != null) abilityQUI.TriggerCooldown(cooldown);
    }

    public void TriggerAbilityECooldown(float cooldown)
    {
        if (abilityEUI != null) abilityEUI.TriggerCooldown(cooldown);
    }

    public void TriggerAbilityRCooldown(float cooldown)
    {
        if (abilityRUI != null) abilityRUI.TriggerCooldown(cooldown);
    }
}