using UnityEngine;
using System.Collections;

public abstract class Combat : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] protected Transform attackPoint;
    protected float attackRange;
    protected float attackDamage;
    protected float attackCooldown;
    protected float lastAttackTime;

    protected abstract void TryAttack();
    protected abstract IEnumerator Attack();

    public virtual void CancelAttack() { }

    public float AttackDamage => attackDamage;

    public void SetDamage(float damage)
    {
        attackDamage = damage;
    }

    public void SetRange(float value)
    {
        attackRange = value;
    }

    public void SetCooldown(float value)
    {
        attackCooldown = value;
    }
}
