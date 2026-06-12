using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour, IHealable, IDamageable, IBuffable, ISaveable
{
    [SerializeField] private StatsData stats;
    [SerializeField] private Animator animator;

    private float _health;
    private PlayerMovement _playerMovement;
    private PlayerCombat _playerCombat;
    private PlayerParry _parry;

    //CHEATS
    private bool _godMode;

    //Propriedades
    public float CurrentHealth => _health;
    public float MaxHealth => stats.maxHealth;
    public float AttackDamage => stats.attackDamage;
    public float AttackRange => stats.attackRange;
    public float AttackCooldown => stats.attackCooldown;
    public float ShieldCooldown => stats.shieldCooldown;
    public float Speed => stats.moveSpeed;
    public float AngularSpeed => stats.angularSpeed;
    public float StunDuration => stats.stunDuration;
    public float KnockbackDistance => stats.knockbackDistance;
    public Animator Animator => animator;

    //Instancias Temporárias
    private bool _isBuff;
    private float _buffDuration;
    private float _buffTimer;
    
    //Eventos
    public event Action OnHealthChanged;
    private void DispatchHealthChanged() => OnHealthChanged?.Invoke();
    public event Action OnBuff;
    private void StatsChanged() => OnBuff?.Invoke();

    private void Awake()
    {
        SaveManager sm = FindFirstObjectByType<SaveManager>();
        if (sm != null) sm.RegisterSaveable(this);

        _playerCombat = GetComponent<PlayerCombat>();
        _playerCombat.SetDamage(stats.attackDamage);
        _playerCombat.SetRange(stats.attackRange);
        _playerCombat.SetCooldown(stats.attackCooldown);
        _playerMovement = GetComponent<PlayerMovement>();
        _playerMovement.SetSpeed(stats.moveSpeed);
        _health = stats.maxHealth;
        _parry = GetComponent<PlayerParry>();
        _parry.SetCooldown(stats.shieldCooldown);
    }

    private void Update()
    {
        if (_isBuff)
        {
            _buffTimer += Time.deltaTime;

            if (_buffTimer >= _buffDuration)
            {
                RemoveBuff();
            }
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

    public void BeingBuff(float duration)
    {
        _isBuff = true;
        _buffTimer = 0;
        _buffDuration = duration;
    }

    public void RemoveBuff()
    {
        _playerCombat.SetDamage(stats.attackDamage);
        _playerCombat.SetCooldown(stats.attackCooldown);
        _playerMovement.SetSpeed(stats.moveSpeed);
        _isBuff = false;
    }

    public void IncrementHealth(float bonusHealth)
    {
        stats.maxHealth += bonusHealth;
        DispatchHealthChanged();
    }

    public void IncrementDamage(float bonusDamage)
    {
        _playerCombat.SetDamage(_playerCombat.AttackDamage+bonusDamage);
    }

    public void MultiplyVelocity(float value)
    {
        _playerMovement.SetSpeed(_playerMovement.Speed*value);
    }

    public void MultiplyAttackCooldown(float value)
    {
        _playerCombat.SetCooldown(_playerCombat.AttackCooldown*value);
    }

    public void DecrementCooldown(float timeToReduce)
    {
        _parry.SetCooldown(_parry.Cooldown - timeToReduce);
    }

    public void SaveTempStats()
    {
        stats.maxHealth = MaxHealth;
        stats.currentHealth = CurrentHealth;
        stats.attackDamage = AttackDamage;
        stats.shieldCooldown = ShieldCooldown;
        stats.moveSpeed = _playerMovement.Speed;
        stats.attackDamage = _playerCombat.AttackDamage;
        stats.attackCooldown = _playerCombat.AttackCooldown;

        StatsChanged();
    }

    //CHEATS
    public void SetGodMode(bool value)
    {
        _godMode = value;
    }

    public void TakeDamage(float damage, Combat combat)
    {
        if (_parry != null && _parry.IsParrying)
        {
            _parry.SucessfulParry(combat);
            return;
        }
        
        //CHEATS
        if (_godMode) return;

        _health -= damage;
        DispatchHealthChanged();
        if (_health <= 0)
        {
            _health = 0;
            Death();
        }
    }

    public string GetSaveID()
    {
        return "PlayerStats";
    }

    public string GetSaveDataAsJson()
    {
        SaveData data = new SaveData
        {
            maxHealth = stats.maxHealth,
            currentHealth = _health,
            moveSpeed = _playerMovement.Speed,
            angularSpeed = stats.angularSpeed,
            attackDamage = _playerCombat.AttackDamage,
            attackRange = stats.attackRange,
            attackCooldown = _playerCombat.AttackCooldown,
            shieldCooldown = _parry.Cooldown,
            stunDuration = stats.stunDuration,
            knockbackDistance = stats.knockbackDistance,
            isBuff = _isBuff,
            buffDuration = _buffDuration,
            buffTimer = _buffTimer
        };
        return JsonUtility.ToJson(data);
    }

    public void LoadFromJson(string json)
    {
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        stats.maxHealth = data.maxHealth;
        stats.currentHealth = data.currentHealth;
        stats.moveSpeed = data.moveSpeed;
        stats.angularSpeed = data.angularSpeed;
        stats.attackDamage = data.attackDamage;
        stats.attackRange = data.attackRange;
        stats.attackCooldown = data.attackCooldown;
        stats.shieldCooldown = data.shieldCooldown;
        stats.stunDuration = data.stunDuration;
        stats.knockbackDistance = data.knockbackDistance;

        _health = data.currentHealth;
        _playerMovement.SetSpeed(data.moveSpeed);
        _playerCombat.SetDamage(data.attackDamage);
        _playerCombat.SetCooldown(data.attackCooldown);
        _parry.SetCooldown(data.shieldCooldown);

        DispatchHealthChanged();
    }

    [Serializable]
    public struct SaveData
    {
        public float maxHealth;
        public float currentHealth;
        public float moveSpeed;
        public float angularSpeed;
        public float attackDamage;
        public float attackRange;
        public float attackCooldown;
        public float shieldCooldown;
        public float stunDuration;
        public float knockbackDistance;
        public bool isBuff;
        public float buffDuration;
        public float buffTimer;
    }
}
