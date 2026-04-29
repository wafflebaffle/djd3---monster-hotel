using UnityEngine;

public abstract class Combat : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] protected float attackRange = 1.5f;
    [SerializeField] protected float attackDamage = 25;
    [SerializeField] protected float attackCooldown = 0.5f;
    [SerializeField] protected Transform attackPoint;
    protected float lastAttackTime;

    protected abstract void TryAttack();
    protected abstract void Attack();

    public abstract void SetDamage(float damage);

}
