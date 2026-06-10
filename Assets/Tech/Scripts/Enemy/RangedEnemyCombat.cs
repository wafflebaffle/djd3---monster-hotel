using System.Collections;
using UnityEngine;

public class RangedEnemyCombat : Combat
{
    //[SerializeField] private float attackTreshold = 1;
    [SerializeField] private Animator attackAnim;
    [SerializeField] private string attackAnimName = "Shot";
    [SerializeField] private float attackAnimDuration = 1.0f;
    [SerializeField] private float attackAngleTolerance = 10.0f;
    [SerializeField] private GameObject projectile;
    [SerializeField] private LayerMask player;
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
        YieldInstruction wfsCooldown = new WaitForSeconds(attackAnimDuration);

        HadAttack = false;

        yield return wfs;

        Instantiate(projectile, attackPoint);
        HadAttack = true;

        yield return wfsCooldown;
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
        float angle = Vector3.Angle(transform.forward, (_enemy.GetTarget().position - transform.position).normalized);
        bool isFacing = angle <= attackAngleTolerance;

        return isFacing && _enemy.DistanceToTarget() <= attackRange;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
