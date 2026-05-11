using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeAbility", menuName = "Upgrades/UpgradeAbility")]
public class UpgradeAbility : Upgrade
{
    [SerializeField] public float decreaseAbilityCooldown;
    public override void Effect(PlayerStats player, GameObject panel)
    {
        player.DecrementCooldown(decreaseAbilityCooldown);
        
        base.Effect(player, panel);
    }
}