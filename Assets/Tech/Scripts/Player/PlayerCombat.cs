using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : Combat
{
    private InputAction _attack;

    public float AttackDamage => attackDamage;
    public float AttackCooldown => attackCooldown;

    [Header("References")]
    [SerializeField] private string AttackInput = "Attack";


    void Start()
    {
        _attack = InputSystem.actions.FindAction(AttackInput);
    }
    void Update()
    {
        
        if (_attack.WasPressedThisFrame())
        {
            Debug.Log("Hi");
            TryAttack();
        }
    }

    protected override void TryAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown)
            return;

        lastAttackTime = Time.time;
        Attack();
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

    public override void SetDamage(float damage)
    {
        attackDamage = damage;
    }
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
    
}