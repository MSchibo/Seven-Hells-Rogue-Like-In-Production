using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class HauptcharakterMovement : MonoBehaviour
{
    [Header("Movement & Attack")]
    public float moveSpeed = 5f;
    public float stopDistance = 0.1f;
    public float attackDuration = 0.5f;
    public float attackCooldown = 2f; // Angepasst auf 2 Sekunden
    public float attackRange = 0.5f; // Muss mit PlayerAttack übereinstimmen

    [Header("Fähigkeiten")]
    public float dodgeCooldown = 2f;
    // Q, E, R sind zunächst deaktiviert
    // public float abilityQCooldown = 5f;
    // public float abilityECooldown = 8f;
    // public float abilityRCooldown = 12f;

    [Header("Effekte & Audio")]
    public GameObject attackEffectPrefabEast;
    public GameObject attackEffectPrefabWest;
    public GameObject pentagramPrefab;
    public AudioClip pentagramSoundClip;
    public AudioClip tutorialThemeClip;
    public AudioClip attackSoundClip;

    public float pentagramAnimationDuration = 2f;

    private Animator animator;
    private PlayerInputActions inputActions;
    private Camera mainCamera;
    private Vector2 targetPosition;
    private bool isMoving;
    private bool isAttacking;
    private float lastAttackTime;
    private Rigidbody2D rb;
    private Vector2 lastAttackDirection;
    private bool isControllable = false;
    private bool hasInitialized = false;
    private Vector2 movementInput;
    private int currentHP = 100; // Basiswert
    private int maxHP = 100;     // Basiswert
    private PlayerAttack playerAttack; // Referenz zum Attack-Script

    void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.Player.Enable();
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogWarning("Rigidbody2D-Komponente nicht gefunden auf " + gameObject.name);
        }
        playerAttack = GetComponent<PlayerAttack>(); // Hole die PlayerAttack-Komponente

        inputActions.Player.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => movementInput = Vector2.zero;

        // inputActions.Player.Attack.performed += ctx => TryManualAttack(); // Leertaste für manuellen Angriff deaktiviert
        inputActions.Player.Dodge.performed += ctx => TryDodge();
        // Fähigkeiten temporär deaktiviert
        // inputActions.Player.AbilityQ.performed += ctx => TryUseAbilityQ();
        // inputActions.Player.AbilityE.performed += ctx => TryUseAbilityE();
        // inputActions.Player.AbilityR.performed += ctx => TryUseAbilityR();
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
        lastAttackTime = -attackCooldown;
        lastAttackDirection = Vector2.right;

        // Maus komplett deaktivieren, außer im Hauptmenü
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (!hasInitialized && SceneManager.GetActiveScene().name == "Start Tutorial")
        {
            StartCoroutine(InitializeGame());
            hasInitialized = true;
        }
        else
        {
            isControllable = true;
            animator.Play("Idle", 0);
        }
    }

    IEnumerator InitializeGame()
    {
        inputActions.Player.Disable();

        if (pentagramPrefab != null)
        {
            Vector3 spawnPosition = transform.position + new Vector3(0, -0.5f, 0);
            GameObject pentagram = Instantiate(pentagramPrefab, spawnPosition, Quaternion.identity);

            AudioSource audioSource = pentagram.AddComponent<AudioSource>();
            if (pentagramSoundClip != null)
            {
                audioSource.clip = pentagramSoundClip;
                audioSource.loop = true;
                audioSource.Play();
            }

            Animator pentagramAnimator = pentagram.GetComponent<Animator>();
            if (pentagramAnimator != null)
            {
                pentagramAnimator.Play("PentagramAnimation", 0);
                yield return new WaitForSeconds(pentagramAnimationDuration);
            }

            if (audioSource.isPlaying)
                audioSource.Stop();

            Destroy(pentagram);
        }

        // HUD-Szene additiv laden
        HUDLoader.Instance.LoadHUDOnce();

        // Warte, bis die HUD-Szene geladen ist
        yield return new WaitUntil(() => HUDLoader.Instance.IsLoaded);

        AudioManager audioManager = FindFirstObjectByType<AudioManager>();
        if (audioManager != null && tutorialThemeClip != null)
        {
            audioManager.PlayMusic(tutorialThemeClip, true);
        }

        inputActions.Player.Enable();
        isControllable = true;
        animator.Play("Idle", 0);
    }

    void Update()
    {
        if (!isControllable || isAttacking) return;

        if (movementInput != Vector2.zero)
        {
            Vector2 moveDir = movementInput.normalized;
            lastAttackDirection = moveDir;
            string walkingState = GetWalkingState(moveDir);
            animator.Play(walkingState, 0);
        }
        else if (!isMoving && movementInput == Vector2.zero)
        {
            string idleState = GetIdleState(lastAttackDirection);
            animator.Play(idleState, 0);
        }
    }

    void FixedUpdate()
    {
        if (isMoving && !isAttacking && isControllable)
        {
            Vector2 currentPosition = rb.position;
            Vector2 moveDirection = (targetPosition - currentPosition).normalized;
            float distanceToTarget = Vector2.Distance(currentPosition, targetPosition);

            if (distanceToTarget > stopDistance)
            {
                rb.MovePosition(currentPosition + moveDirection * moveSpeed * Time.fixedDeltaTime);
                animator.Play(GetWalkingState(moveDirection), 0);
            }
            else
            {
                isMoving = false;
                animator.Play(GetIdleState(lastAttackDirection), 0);
            }
        }
        else if (!isMoving && !isAttacking && movementInput != Vector2.zero)
        {
            Vector2 moveDir = movementInput.normalized;
            rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);
        }

        // Automatischer Angriff, synchronisiert mit PlayerAttack-Cooldown
        if (!isAttacking && Time.time >= lastAttackTime + attackCooldown && isControllable && playerAttack != null && playerAttack.CanAttack())
        {
            TryAutoAttack();
        }
    }

    void TryAutoAttack()
    {
        // Überprüfe Gegner im attackRange mit Überlappung
        Collider2D[] hits = Physics2D.OverlapCircleAll(rb.position, attackRange, LayerMask.GetMask("Enemy"));
        if (hits.Length > 0)
        {
            // Nächstgelegener Gegner
            Collider2D nearest = hits[0];
            float minDistance = Vector2.Distance(rb.position, nearest.transform.position);
            foreach (Collider2D hit in hits)
            {
                float distance = Vector2.Distance(rb.position, hit.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = hit;
                }
            }

            MaleDamned target = nearest.GetComponent<MaleDamned>();
            if (target != null)
            {
                Vector2 direction = (Vector2)nearest.transform.position - rb.position; // Konvertiere Vector3 zu Vector2
                direction = direction.normalized;
                string attackState = GetAttackingState(direction);
                if (attackState != null)
                {
                    isAttacking = true;
                    lastAttackDirection = direction;
                    animator.Play(attackState, 0);
                    SpawnAttackEffect(direction);
                    lastAttackTime = Time.time;

                    if (playerAttack != null)
                    {
                        playerAttack.PerformAttack(target);
                    }

                    StartCoroutine(AttackCoroutine());
                }
            }
        }
    }

    // void TryManualAttack()
    // {
    //     if (!isControllable || isAttacking || Time.time < lastAttackTime + attackCooldown || playerAttack == null || !playerAttack.CanAttack())
    //         return;

    //     string attackState = GetAttackingState(lastAttackDirection);
    //     if (attackState != null)
    //     {
    //         isAttacking = true;
    //         animator.Play(attackState, 0);
    //         SpawnAttackEffect(lastAttackDirection);
    //         lastAttackTime = Time.time;

    //         // Raycast in die letzte Bewegungsrichtung
    //         RaycastHit2D hit = Physics2D.Raycast(rb.position, lastAttackDirection, attackRange, LayerMask.GetMask("Enemy"));
    //         if (hit.collider != null)
    //         {
    //             MaleDamned target = hit.collider.GetComponent<MaleDamned>();
    //             if (target != null && playerAttack != null)
    //             {
    //                 playerAttack.PerformAttack(target);
    //             }
    //         }

    //         StartCoroutine(AttackCoroutine());
    //         if (HUDManager.Instance != null)
    //         {
    //             HUDManager.Instance.StartAttackCooldown(attackCooldown);
    //         }
    //     }
    // }

    void SpawnAttackEffect(Vector2 direction)
    {
        AudioSource.PlayClipAtPoint(attackSoundClip, transform.position);

        GameObject effectPrefab = direction.x > 0 ? attackEffectPrefabEast : attackEffectPrefabWest;
        if (effectPrefab != null)
        {
            // Konvertiere rb.position zu Vector3 und skaliere direction.normalized
            Vector3 spawnPos = new Vector3(rb.position.x, rb.position.y, 0) + (Vector3)(direction.normalized * 0.5f);
            GameObject effect = Instantiate(effectPrefab, spawnPos, Quaternion.identity);
            Animator effectAnimator = effect.GetComponent<Animator>();
            if (effectAnimator != null)
            {
                effectAnimator.Play(direction.x > 0 ? "AttackAnimation East" : "AttackAnimation West", 0);
                Destroy(effect, attackDuration);
            }
        }
    }

    private IEnumerator AttackCoroutine()
    {
        yield return new WaitForSeconds(attackDuration);
        isAttacking = false;
        yield break;
    }

    void TryDodge()
    {
        if (HUDManager.Instance != null)
        {
            HUDManager.Instance.TriggerDodgeCooldown(dodgeCooldown);
        }
    }

    // Fähigkeiten temporär deaktiviert
    // void TryUseAbilityQ() { }
    // void TryUseAbilityE() { }
    // void TryUseAbilityR() { }

    string GetWalkingState(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            return direction.x > 0 ? "Walking East" : "Walking West";
        else
            return direction.y > 0 ? "Walking North" : "Walking South";
    }

    string GetIdleState(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            return direction.x > 0 ? "Idle" : "Idle West";
        else
            return direction.y > 0 ? "Idle" : "Idle";
    }

    string GetAttackingState(Vector2 direction)
    {
        float angle = Vector2.SignedAngle(Vector2.right, direction);
        if (angle >= -90 && angle <= 90) return "Attacking East";
        if (angle < -90 || angle > 90) return "Attacking West";
        return null;
    }

    void OnDestroy()
    {
        inputActions.Player.Disable();
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP < 0) currentHP = 0;
        if (HUDManager.Instance != null)
        {
            HUDManager.Instance.UpdateHP(currentHP, maxHP);
        }
    }

    public void Heal(int healAmount)
    {
        currentHP += healAmount;
        if (currentHP > maxHP) currentHP = maxHP;
        if (HUDManager.Instance != null)
        {
            HUDManager.Instance.UpdateHP(currentHP, maxHP);
        }
    }

    // Gizmo für Debug-Zwecke
    void OnDrawGizmosSelected()
    {
        if (rb != null) // Null-Check hinzufügen
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(rb.position, attackRange);
        }
    }
}










