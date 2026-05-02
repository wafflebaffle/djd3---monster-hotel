using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeDamage", menuName = "Upgrades/UpgradeDamage")]
public class UpgradeDamage : Upgrade
{
    [field: SerializeField] public float AddDamage { get; private set; }
    public override void Effect(PlayerStats player, GameObject panel)
    {
        player.IncrementDamage(AddDamage);
        
        base.Effect(player, panel);
    }
}