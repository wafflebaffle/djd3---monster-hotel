using System;
using UnityEngine;

public class Spicy : MonoBehaviour
{
    [SerializeField] private BuffsData spicyStats;
    private float damageBuffAmount, velocityBuff, attackCooldownBuff, duration, destroyTimer = 1;
    private void Start()
    {
        damageBuffAmount = spicyStats.damageBuffAmount;
        velocityBuff = spicyStats.velocityBuff;
        attackCooldownBuff = spicyStats.attackCooldownBuff;
        duration = spicyStats.duration;
        destroyTimer = spicyStats.destroyTimer;
    }

    void OnTriggerEnter(Collider other)
    {
        IBuffable target = other.GetComponent<IBuffable>();

        if (target != null)
        {
            target.BeingBuff(duration);
            target.IncrementDamage(damageBuffAmount);
            target.MultiplyVelocity(velocityBuff);
            target.MultiplyAttackCooldown(attackCooldownBuff);

            Destroy(gameObject, destroyTimer);
        }
    }
}
