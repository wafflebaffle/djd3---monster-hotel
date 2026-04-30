using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : Combat
{
    private InputAction _attack;
    private IDamageable _stats;

    public float AttackDamage => attackDamage;
    public float AttackCooldown => attackCooldown;

    [Header("References")]
    [SerializeField] private string AttackInput = "Attack";


    void Start()
    {
        _attack = InputSystem.actions.FindAction(AttackInput);
        _stats = GetComponent<PlayerStats>();
    }
    void Update()
    {
        
        if (_attack.WasPressedThisFrame())
        {
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
        Vector3 directionToTarget;
        Vector3 closestTargetPosition = Vector3.zero;
        IDamageable closestTarget = null;
        float closestDistance = Mathf.Infinity;

        
        Collider[] hits = Physics.OverlapSphere(
            attackPoint.position,
            attackRange
        );

        foreach (Collider hit in hits)
        {
            if (hit.TryGetComponent<IDamageable>(out IDamageable damageableTarget))
            {  
                if(damageableTarget == _stats) continue;

                float distance = Vector3.Distance(hit.transform.position, transform.position);
        
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = damageableTarget;
                    closestTargetPosition = hit.transform.position;
                }
            }
        }

        if (closestTarget != null)
        {
            directionToTarget = (closestTargetPosition - transform.position).normalized;
            float dot = Vector3.Dot(transform.forward, directionToTarget);

            if (dot > 0.5f) // (0.5 ≈ 60)
            {
                closestTarget.TakeDamage(attackDamage, this);
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