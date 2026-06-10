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
        _isBuff = false;

        // Fazer um if para checkar se os valores 
        // do player prefs são zero. 
        // Se não, vai buscar os valores lá.

        //else
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
        return name + ":" + GetType().Name;
    }

    public object GetSaveData()
    {
        SaveData saveData;

        saveData.maxHealth = MaxHealth;
        saveData.currentHealth = CurrentHealth;
        saveData.moveSpeed = Speed;
        saveData.angularSpeed = AngularSpeed;
        saveData.attackDamage = AttackDamage;
        saveData.attackRange = AttackRange;
        saveData.attackCooldown = AttackCooldown;
        saveData.shieldCooldown = ShieldCooldown;
        saveData.stunDuration = StunDuration;
        saveData.knockbackDistance = KnockbackDistance;
        saveData.animationTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        saveData.animationState = animator.GetCurrentAnimatorStateInfo(0).shortNameHash;

        return saveData;
    }

    public void LoadSaveData(object data)
    {
        SaveData saveData = (SaveData)data;

        stats.maxHealth = saveData.maxHealth;
        stats.currentHealth = saveData.currentHealth;
        stats.moveSpeed = saveData.moveSpeed;
        stats.angularSpeed = saveData.angularSpeed;
        stats.attackDamage = saveData.attackDamage;
        stats.attackRange = saveData.attackRange;
        stats.attackCooldown = saveData.attackCooldown;
        stats.shieldCooldown = saveData.shieldCooldown;
        stats.stunDuration = saveData.stunDuration;
        stats.knockbackDistance = saveData.knockbackDistance;
        
        animator.Play(saveData.animationState, 0, saveData.animationTime);

        DispatchHealthChanged();
    }

    [System.Serializable]
    private struct SaveData
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
        public float    animationTime;
        public int    animationState;
    }
}
