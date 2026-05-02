using UnityEngine;

public abstract class Upgrade : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    public virtual void Effect(PlayerStats player, GameObject panel)
    {
        player.SaveTempStats();
        Time.timeScale = 1;
        panel.SetActive(false);
    }
}