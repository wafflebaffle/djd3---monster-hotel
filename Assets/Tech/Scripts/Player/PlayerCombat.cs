using UnityEngine;

public class PlayerMelee : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private int attackDamage = 25;
    [SerializeField] private float attackCooldown = 0.5f;

    [Header("References")]
    [SerializeField] private Transform attackPoint;

    private float lastAttackTime;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryAttack();
        }
    }

    private void TryAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown)
            return;

        lastAttackTime = Time.time;
        Attack();
    }

    private void Attack()
    {
        Collider[] hits = Physics.OverlapSphere(
            attackPoint.position,
            attackRange
        );

        foreach (Collider hit in hits)
        {
            // Skip self
            if (hit.transform == transform) continue;
            IDamageable damageable = hit.GetComponent<IDamageable>();

            if (damageable != null)
            {
                damageable.TakeDamage(attackDamage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}