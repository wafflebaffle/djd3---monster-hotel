using UnityEngine;

public abstract class Upgrade : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    public abstract void Effect(PlayerStats player);
}