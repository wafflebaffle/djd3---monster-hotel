using UnityEngine;

public class EnemyCombat : Combat
{
    [SerializeField] private float attackTreshold = 1;
    private EnemyStats _enemy;

    private void Start()
    {
        _enemy = GetComponent<EnemyStats>();
    }

    private void Update()
    {
        if (_enemy.DistanceToTarget() <= attackTreshold)
        {
            TryAttack();
        }
    }

    public override void SetDamage(float damage)
    {
        attackDamage = damage;
    }

    protected override void Attack()
    {
        Collider[] hits = Physics.OverlapSphere(
            attackPoint.position,
            attackRange
        );

        foreach (Collider hit in hits)
        {
            if (hit.transform == transform) continue;

            Vector3 directionToTarget = (hit.transform.position - transform.position).normalized;
            float dot = Vector3.Dot(transform.forward, directionToTarget);

            if (dot > 0.5f) // (0.5 ≈ 60)
            {
                IDamageable damageable;

                if (hit.TryGetComponent<IDamageable>(out damageable))
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
        Attack();
    }
}
