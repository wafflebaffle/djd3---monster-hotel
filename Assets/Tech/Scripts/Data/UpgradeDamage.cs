using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeDamage", menuName = "Upgrades/UpgradeDamage")]
public class UpgradeDamage : Upgrade
{
    [SerializeField] private float addDamage;
    public override void Effect(PlayerStats player, GameObject panel)
    {
        player.IncrementDamage(addDamage);
        
        base.Effect(player, panel);
    }
}