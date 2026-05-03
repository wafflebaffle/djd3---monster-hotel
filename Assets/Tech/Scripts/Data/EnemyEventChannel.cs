using UnityEngine;
using System;

[CreateAssetMenu(fileName = "EnemyEventChannel", menuName = "Events/EnemyEventChannel")]
public class EnemyEventChannel : ScriptableObject
{
    private event Action _onEnemySpawned;
    private event Action _onEnemyDied;

    public event Action OnEnemySpawned
    {
        add => _onEnemySpawned += value;
        remove => _onEnemySpawned -= value;
    }

    public event Action OnEnemyDied
    {
        add => _onEnemyDied += value;
        remove => _onEnemyDied -= value;
    }
    
    public void RaiseEnemySpawned() => _onEnemySpawned?.Invoke();
    public void RaiseEnemyDied() => _onEnemyDied?.Invoke();
}