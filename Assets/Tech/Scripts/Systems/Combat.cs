using UnityEngine;

public abstract class Combat : MonoBehaviour
{
    protected abstract void TryAttack();
    protected abstract void Attack();

    public abstract void SetDamage(int damage);

}
