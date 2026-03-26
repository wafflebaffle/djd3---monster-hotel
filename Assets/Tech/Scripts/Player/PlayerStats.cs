using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour, IHealable
{
    [SerializeField] private StatsData stats;
    private float _health;
    private PlayerMovement _playerMovement;
    private PlayerCombat _playerCombat;
    private bool _canHeal;

    //Propriedades
    public float CurrentHealth => _health;
    public float MaxHealth => stats.maxHealth;
    public int AttackDamage => stats.attackDamage;
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
        // Fazer um if para checkar se os valores 
        // do player prefs são zero. 
        // Se não, vai buscar os valores lá.

        //else
        _playerCombat = GetComponent<PlayerCombat>();
        _playerCombat.SetDamage(stats.attackDamage);
        _playerMovement = GetComponent<PlayerMovement>();
        _playerMovement.SetSpeed(stats.moveSpeed);
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

    public void Heal(float heal)
    {
        _health += heal;
        DispatchHealthChanged();
        if(_health >= stats.maxHealth)
        {
            _health = stats.maxHealth;
        }
    }

    private void Death()
    {
        //Play death animation;
        //for tests purposes
        gameObject.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public bool CanHeal()
    {
        return _health < stats.maxHealth;
    }

    public void IncrementHealth(float bonusHealth)
    {
        stats.maxHealth += bonusHealth;
        DispatchHealthChanged();
    }

    public void IncrementDamage(int bonusDamage)
    {
        stats.attackDamage += bonusDamage;
    }

    public void MultiplyVelocity(float value)
    {
        _playerMovement.SetSpeed(_playerMovement.Speed*value);
    }

    public void DecrementCooldown(int timeToReduce)
    {
        stats.cooldownReduction += timeToReduce;
    }

    public void SaveStats()
    {
        //Send to PlayerPrefs
    }

    public void SaveTempStats()
    {
        stats.maxHealth = MaxHealth;
        stats.currentHealth = CurrentHealth;
        stats.attackDamage = AttackDamage;
        stats.cooldownReduction = CooldownReduction;
        stats.moveSpeed = _playerMovement.Speed;
    }
}
