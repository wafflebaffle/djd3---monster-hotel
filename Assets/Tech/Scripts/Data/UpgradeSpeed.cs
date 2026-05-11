using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeSpeed", menuName = "Upgrades/UpgradeSpeed")]
public class UpgradeSpeed : Upgrade
{
    [SerializeField] public float multiplyVelocity;
    public override void Effect(PlayerStats player, GameObject panel)
    {
        player.MultiplyVelocity(multiplyVelocity);

        base.Effect(player, panel);
    }
}