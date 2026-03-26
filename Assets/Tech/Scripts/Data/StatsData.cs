using UnityEngine;

[CreateAssetMenu(fileName = "StatsData", menuName = "Scriptable Objects/StatsData")]
public class StatsData : ScriptableObject
{
    public float maxHealth;
    public float currentHealth;
    public float moveSpeed;
    public int attackDamage;
    public float cooldownReduction;
}