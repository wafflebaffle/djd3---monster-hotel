using System.Collections;
using UnityEngine;

public class EnemyCombat : Combat
{
    [SerializeField] private Animator attackAnim;
    [SerializeField] private string attackAnimName = "Punch";
    [SerializeField] private float attackAnimDuration = 1.0f;
    [SerializeField] private float stunDuration = 1.0f;
    private EnemyStats _enemy;
    private EnemyMovement _movement;

    private bool _isStunned;
    public bool HadAttack { get; private set; }
    public bool IsStunned => _isStunned;

    private void Start()
    {
        _enemy = GetComponent<EnemyStats>();
        _movement = GetComponent<EnemyMovement>();
    }

    public void DoAttack()
    {
        if(_isStunned) return;
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
            if (hit.TryGetComponent<PlayerParry>(out PlayerParry parry))
            {
               if (parry.IsParrying)
               {
                    parry.SucessfulParry(this);

                    HadAttack = true;
                    yield break;
               }
            }

            IDamageable damageable;

            if (hit.TryGetComponent<IDamageable>(out damageable))
            {
                //Vector3 directionToTarget = (hit.transform.position - transform.position).normalized; 
                damageable.TakeDamage(attackDamage, this);
            }
        }
    }

    protected override void TryAttack()
    {
        if (_isStunned) return;

        if (Time.time < lastAttackTime + attackCooldown)
            return;

        lastAttackTime = Time.time;

        attackAnim.SetTrigger(attackAnimName);
        StartCoroutine(Attack());
    }

    public bool CanAttack()
    {
        return _enemy.GetTarget() && _enemy.DistanceToTarget() <= attackRange;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    public void RequestStun()
    {
        _isStunned = true;

        if (_movement != null)
        {
            _movement.Stun();
        }
      
        StartCoroutine(StunRoutine());
    }

    private IEnumerator StunRoutine()
    {
        yield return new WaitForSeconds(stunDuration);
        _isStunned = false;
        _movement.Unstun();
    }


    public bool EndStun()
    {
        return !_isStunned;
    }

}
