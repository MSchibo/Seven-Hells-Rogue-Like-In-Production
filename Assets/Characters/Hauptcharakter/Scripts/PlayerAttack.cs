using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float damage = 7f; // Anpassbarer Schaden (7-10 empfohlen)
    [SerializeField] private float attackRange = 0.5f; // Reichweite des Angriffs, steuert Collider-Größe
    [SerializeField] private float attackCooldown = 2f; // Angriff alle 2 Sekunden (0.5 APS)

    private bool canAttack = true;

    void Start()
    {
        // Passe den Collider-Bereich an attackRange an
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            collider.size = new Vector2(attackRange, attackRange); // Quadratischer Bereich
        }
    }

    // Kollisionsschaden entfernt, Schaden wird jetzt manuell über TryAttack aufgerufen
    public void PerformAttack(MaleDamned target)
    {
        if (canAttack && target != null)
        {
            bool isCrit = Random.value < 0.25f; // 25% Crit Chance
            float finalDamage = isCrit ? damage * 1.5f : damage;

            target.TakeDamage(finalDamage, transform.position, isCrit);
            StartCoroutine(AttackCooldown());
        }
    }

    System.Collections.IEnumerator AttackCooldown()
    {
        canAttack = false;
        if (HUDManager.Instance != null)
        {
            HUDManager.Instance.StartAttackCooldown(attackCooldown);
            Debug.Log("AttackCooldown gestartet über HUDManager: " + attackCooldown + " Sekunden");
        }
        else
        {
            Debug.LogWarning("HUDManager.Instance ist null, Cooldown nicht gestartet!");
        }
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    // Methode, um den Cooldown-Status abzufragen (für Synchronisation mit HauptcharakterMovement)
    public bool CanAttack()
    {
        return canAttack;
    }
}
