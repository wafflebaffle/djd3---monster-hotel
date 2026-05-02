using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeSpeed", menuName = "Upgrades/UpgradeSpeed")]
public class UpgradeSpeed : Upgrade
{
    [field: SerializeField] public float MultiplyVelocity { get; private set; }
    public override void Effect(PlayerStats player)
    {
        player.MultiplyVelocity(MultiplyVelocity);
    }
}