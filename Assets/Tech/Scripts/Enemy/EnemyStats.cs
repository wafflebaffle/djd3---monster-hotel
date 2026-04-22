using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyStats : MonoBehaviour
{
    [SerializeField] private StatsData stats;
    private float _health;
    private EnemyMovement _enemyMovement;
    private EnemySight _enemySight;

    //Propriedades
    public float CurrentHealth => _health;
    public float MaxHealth => stats.maxHealth;
    public int AttackDamage => stats.attackDamage;
    public float CooldownReduction => stats.cooldownReduction;
    public float Speed => stats.moveSpeed;
    public float AngularSpeed => stats.angularSpeed;
    
    //Eventos
    public event Action OnHealthChanged;

    private void DispatchHealthChanged()
    {
        OnHealthChanged?.Invoke();
    }

    private void Awake()
    {
        _enemyMovement = GetComponent<EnemyMovement>();
        _enemySight = GetComponent<EnemySight>();
        _enemyMovement.SetSpeed(stats.moveSpeed);
        _enemyMovement.SetAngularSpeed(stats.angularSpeed);
        _health = stats.maxHealth;
    }

    public void TakeDamage(float damage)
    {
        _health -= damage;
        //_enemySight.SetTarget()
        
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

    public Transform GetTarget()
    {
        return _enemySight.Target;
    }
}