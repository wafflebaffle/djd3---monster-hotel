public interface IBuffable
{
    public void IncrementHealth(float bonusHealth);
    public void IncrementDamage(float bonusDamage);
    public void MultiplyVelocity(float value);
    public void MultiplyAttackCooldown(float value);
    public void BeingBuff(float duration);
}