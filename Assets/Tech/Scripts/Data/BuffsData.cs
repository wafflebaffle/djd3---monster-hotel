using UnityEngine;

[CreateAssetMenu(fileName = "BuffsData", menuName = "Scriptable Objects/BuffsData")]
public class BuffsData : ScriptableObject
{
    public float damageBuffAmount, velocityBuff, attackCooldownBuff, duration, destroyTimer;
}