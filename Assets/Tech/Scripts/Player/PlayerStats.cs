using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour, IHealable
{
    [SerializeField] private StatsData stats;
    private float _maxHealth;
    private float _health;
    //For Damage
    private int _powerLevel;
    //To reduce second ability
    private float _cooldownReduction ;
    private PlayerMovement _playerMovement;
    private bool canHeal;

    //Propriedades
    public float CurrentHealth => _health;
    public float MaxHealth => _maxHealth;
    public int PowerLevel => _powerLevel;
    public float CooldownReduction => _cooldownReduction;

    public float Speed => stats.moveSpeed;

    private void Awake()
    {
        // Fazer um if para checkar se os valores 
        // do player prefs são zero. 
        // Se não, vai buscar os valores lá.

        //else

        _playerMovement = GetComponent<PlayerMovement>();

        UpdateTempStats();

        _health = _maxHealth;
    }

    public void TakeDamage(float damage)
    {
        _health -= damage;
        if(_health <= 0)
        {
            _health = 0;
            Death();
        }
    }

    public void Heal(float heal)
    {
        _health += heal;
        if(_health >= _maxHealth)
        {
            _health = _maxHealth;
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
        return _health < _maxHealth;
    }

    public void IncrementHealth(float bonusHealth)
    {
        _maxHealth += bonusHealth;
    }

    public void IncrementDamage(int bonusDamage)
    {
        _powerLevel += bonusDamage;
    }

    public void MultiplyVelocity(float value)
    {
        _playerMovement.SetSpeed(_playerMovement.Speed*value);
    }

    public void DecrementCooldown(int timeToReduce)
    {
        _cooldownReduction += timeToReduce;
    }

    public void SaveStats()
    {
        //Send to PlayerPrefs
    }

    private void UpdateTempStats()
    {
        _maxHealth = stats.maxHealth;
        _powerLevel = stats.powerLevel;
        _cooldownReduction = stats.cooldownReduction;
        _playerMovement.SetSpeed(stats.moveSpeed);
    }

    public void SaveTempStats()
    {
        stats.maxHealth = MaxHealth;
        stats.currentHealth = CurrentHealth;
        stats.powerLevel = PowerLevel;
        stats.cooldownReduction = CooldownReduction;
        stats.moveSpeed = _playerMovement.Speed;
    }
}
