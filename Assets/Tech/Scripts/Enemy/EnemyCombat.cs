using NUnit.Framework;
using UnityEngine;

public class EnemyCombat : Combat
{
    [SerializeField] private float attackTreshold = 1;
    [SerializeField] private Animator attackAnim;
    [SerializeField] private string attackAnimName = "Punch";
    private EnemyStats _enemy;
    public bool HasAttack { get; private set; }

    private void Start()
    {
        _enemy = GetComponent<EnemyStats>();
        HasAttack = false;
    }

    public void DoAttack()
    {
        TryAttack();
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
                //Vector3 directionToTarget = (hit.transform.position - transform.position).normalized;

                damageable.TakeDamage(attackDamage, this);
            }
        }

        HasAttack = true;
    }

    protected override void TryAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown)
            return;

        lastAttackTime = Time.time;
        attackAnim.SetTrigger(attackAnimName);
        Attack();
    }

    public bool CanAttack()
    {
        return _enemy.GetTarget() && _enemy.DistanceToTarget() <= attackTreshold;
    }

    public void IdkIJustWantToTurnThisOff()
    {
        HasAttack = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
