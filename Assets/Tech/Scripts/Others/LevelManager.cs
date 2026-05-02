using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private EnemyEventChannel enemyEvent;
    [SerializeField] private RandomUpgradeSelector upgradePanel;
    private int _enemiesAlive;
    private void EnemyHasSpawn() => _enemiesAlive++;
    private void EnemyHasDied() => _enemiesAlive--;

    private void OnEnable()
    {
        enemyEvent.OnEnemySpawned += EnemyHasSpawn;
        enemyEvent.OnEnemyDied += EnemyHasDied;
    }

    private void OnDisable()
    {
        enemyEvent.OnEnemySpawned -= EnemyHasSpawn;
        enemyEvent.OnEnemyDied -= EnemyHasDied;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_enemiesAlive == 0)
            if(other.TryGetComponent(out PlayerStats player))
            {
                player.RemoveBuff();
                upgradePanel.SetPlayer(player);
                upgradePanel.gameObject.SetActive(true);
            }
    }

    private void Update()
    {
        Debug.Log(_enemiesAlive);
    }
}
