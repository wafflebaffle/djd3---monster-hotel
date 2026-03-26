using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyStats : MonoBehaviour
{
    [SerializeField] private StatsData stats;
    private float _health;
    private EnemyMovement _enemyMovement;

    //Propriedades
    public float CurrentHealth => _health;
    public float MaxHealth => stats.maxHealth;
    public int PowerLevel => stats.powerLevel;
    public float CooldownReduction => stats.cooldownReduction;
    public float Speed => stats.moveSpeed;
    
    //Eventos
    public event Action OnHealthChanged;

    private void DispatchHealthChanged()
    {
        OnHealthChanged?.Invoke();
    }

    private void Awake()
    {
        _enemyMovement = GetComponent<EnemyMovement>();
        _enemyMovement.SetSpeed(stats.moveSpeed);
        _health = stats.maxHealth;
    }

    public void TakeDamage(float damage)
    {
        _health -= damage;
        DispatchHealthChanged();
        if(_health <= 0)
        {
            _health = 0;
            Death();
        }
    }
    private void Death()
    {
        //Play death animation;
        //for tests purposes
        gameObject.SetActive(false);
    }
}