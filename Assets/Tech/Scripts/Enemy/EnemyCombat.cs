using UnityEngine;

public class EnemyCombat : Combat
{
    [SerializeField] private float attackTreshold = 1;
    [SerializeField] private Animator attackAnim;
    [SerializeField] private string attackAnimName = "Punch";
    private EnemyStats _enemy;

    private void Start()
    {
        _enemy = GetComponent<EnemyStats>();
    }

    private void Update()
    {
        if (_enemy.GetTarget() && _enemy.DistanceToTarget() <= attackTreshold)
        {
            TryAttack();
        }
    }

    protected override void Attack()
    {
        Collider[] hits = Physics.OverlapSphere(
            attackPoint.position,
            attackRange
        );

        foreach (Collider hit in hits)
        {
            IDamageable damageable;

            if (hit.TryGetComponent<IDamageable>(out damageable))
            {
            Vector3 directionToTarget = (hit.transform.position - transform.position).normalized;
            float dot = Vector3.Dot(transform.forward, directionToTarget);

            if (dot > 0.5f) // (0.5 ≈ 60)
            {
                damageable.TakeDamage(attackDamage, this);
            }
            }
        }
    }

    protected override void TryAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown)
            return;

        lastAttackTime = Time.time;
        attackAnim.SetTrigger(attackAnimName);
        Attack();
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
