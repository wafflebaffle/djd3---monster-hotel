using UnityEngine;

[CreateAssetMenu(fileName = "StatsData", menuName = "Scriptable Objects/StatsData")]
public class StatsData : ScriptableObject
{
    public float maxHealth;
    public float currentHealth;
    public float moveSpeed;
    public float angularSpeed;
    public float attackDamage;
    public float attackSpeed;
    public float cooldownReduction;
}