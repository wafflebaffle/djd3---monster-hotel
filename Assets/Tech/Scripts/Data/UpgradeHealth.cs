using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeHealth", menuName = "Upgrades/UpgradeHealth")]
public class UpgradeHealth : Upgrade
{
    [SerializeField] private float addMaxHealth;
    public override void Effect(PlayerStats player, GameObject panel)
    {
        player.IncrementDamage(addMaxHealth);
        
        base.Effect(player, panel);
    }
}