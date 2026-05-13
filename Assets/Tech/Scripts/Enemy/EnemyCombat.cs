using System.Collections;
using UnityEngine;

public class EnemyCombat : Combat
{
    [SerializeField] private float attackTreshold = 1;
    [SerializeField] private Animator attackAnim;
    [SerializeField] private string attackAnimName = "Punch";
    [SerializeField] private float attackAnimDuration = 1.0f;
    private EnemyStats _enemy;
    public bool HadAttack { get; private set; }

    private void Start()
    {
        _enemy = GetComponent<EnemyStats>();
    }

    public void DoAttack()
    {
        TryAttack();
    }

    protected override IEnumerator Attack()
    {
        YieldInstruction wfs = new WaitForSeconds(attackAnimDuration);

        yield return wfs;
        
        HadAttack = false;

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

        HadAttack = true;
    }

    protected override void TryAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown)
            return;

        lastAttackTime = Time.time;
        attackAnim.SetTrigger(attackAnimName);
        StartCoroutine(Attack());
    }

    public bool CanAttack()
    {
        return _enemy.GetTarget() && _enemy.DistanceToTarget() <= attackTreshold;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
