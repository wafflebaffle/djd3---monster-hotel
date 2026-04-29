using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : Combat
{
    private CharacterController _controller;
    private InputAction _attack;

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 1.5f;
    private int attackDamage = 25;
    [SerializeField] private float attackCooldown = 0.5f;

    public float AttackDamage => attackDamage;
    public float AttackCooldown => attackCooldown;

    [Header("References")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private string AttackInput = "Attack";

    private float lastAttackTime;


    void Start()
    {
        _controller = GetComponent<CharacterController>();

        _attack = InputSystem.actions.FindAction(AttackInput);
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

    public override void SetDamage(int damage)
    {
        attackDamage = damage;
    }

    private void OnDrawGizmosSelected()
    {
       
        
    }
}