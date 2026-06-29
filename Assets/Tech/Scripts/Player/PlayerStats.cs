using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages player statistics, including health, damage, movement, buffs, and state persistence.
/// </summary>
/// <remarks>Implements interfaces for healing, taking damage, applying buffs, and saving or loading player state.
/// Integrates with player movement, combat, and parry systems, and supports event-driven updates for health and
/// buffs.</remarks>
public class PlayerStats : MonoBehaviour, IHealable, IDamageable, IBuffable, ISaveable
{
    /// <summary>
    /// Stores the statistics data for the associated entity.
    /// </summary>
    [SerializeField] private StatsData stats;
    /// <summary>
    /// Controls the animation of the associated GameObject.
    /// </summary>
    [SerializeField] private Animator animator;
    /// <summary>
    /// Specifies the trigger name used to initiate the take damage animation.
    /// </summary>
    [SerializeField] private string triggerTakeDamage = "TakeDamage";
    /// <summary>
    /// Specifies the animation trigger name for the die event.
    /// </summary>
    [SerializeField] private string triggerDie = "Die";
    /// <summary>
    /// Timer for death animation
    /// </summary>
    [SerializeField] private float deathTimer = 1.0f;

    [SerializeField] private float invulnerabilityDuration = 1.0f;

    [SerializeField] private GameObject deathScreenUI;

    /// <summary>
    /// Represents the health value of the player.
    /// </summary>
    private float _health;
    private PlayerMovement _playerMovement;
    private PlayerCombat _playerCombat;
    private PlayerParry _parry;

    private bool _isDead;
    public bool IsDead => _isDead;
    private bool _isInvulnerable;

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

        _playerMovement = GetComponent<PlayerMovement>();
        _playerCombat = GetComponent<PlayerCombat>();
        _parry = GetComponent<PlayerParry>();

        if(sm != null && sm.HasSavedGame()) return;

        _playerCombat.SetDamage(stats.attackDamage);
        _playerCombat.SetRange(stats.attackRange);
        _playerCombat.SetCooldown(stats.attackCooldown);
        _playerMovement.SetSpeed(stats.moveSpeed);
        _health = stats.maxHealth;
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


    /// <summary>
    /// Method that increments player current health value
    /// </summary>
    /// <param name="heal"></param>
    public void Heal(float heal)
    {
        _health += heal;
        DispatchHealthChanged();
        if(_health >= stats.maxHealth)
        {
            _health = stats.maxHealth;
        }
    }

    /// <summary>
    /// Determines whether the player can be healed based on current and maximum health values.
    /// </summary>
    /// <returns>true if the current health is less than the maximum health; otherwise, false.</returns>
    public bool CanHeal()
    {
        return _health < stats.maxHealth;
    }

    /// <summary>
    /// Activates a buff effect for a specified duration.
    /// </summary>
    /// <param name="duration">Duration of the buff in seconds.</param>
    public void BeingBuff(float duration)
    {
        _isBuff = true;
        _buffTimer = 0;
        _buffDuration = duration;
    }

    /// <summary>
    /// Removes the active buff from the player, restoring attack damage, attack cooldown, and movement speed to their
    /// base values.
    /// </summary>
    public void RemoveBuff()
    {
        _playerCombat.SetDamage(stats.attackDamage);
        _playerCombat.SetCooldown(stats.attackCooldown);
        _playerMovement.SetSpeed(stats.moveSpeed);
        _isBuff = false;
    }

    /// <summary>
    /// Increases the maximum health by the specified amount.
    /// </summary>
    /// <param name="bonusHealth">The amount to add to the maximum health.</param>
    public void IncrementHealth(float bonusHealth)
    {
        stats.maxHealth += bonusHealth;
        DispatchHealthChanged();
    }

    /// <summary>
    /// Increases the player's attack damage by the specified bonus amount.
    /// </summary>
    /// <param name="bonusDamage">The additional damage to add to the player's current attack damage.</param>
    public void IncrementDamage(float bonusDamage)
    {
        _playerCombat.SetDamage(_playerCombat.AttackDamage+bonusDamage);
    }

    /// <summary>
    /// Multiplies the player's movement speed by a specified factor.
    /// </summary>
    /// <param name="value">The factor by which to multiply the current speed.</param>
    public void MultiplyVelocity(float value)
    {
        _playerMovement.SetSpeed(_playerMovement.Speed*value);
    }

    /// <summary>
    /// Multiplies the player's attack cooldown by the specified factor.
    /// </summary>
    /// <param name="value">The factor to apply to the current attack cooldown.</param>
    public void MultiplyAttackCooldown(float value)
    {
        _playerCombat.SetCooldown(_playerCombat.AttackCooldown*value);
    }

    /// <summary>
    /// Reduces the cooldown duration by the specified amount.
    /// </summary>
    /// <param name="timeToReduce">The amount of time to subtract from the current cooldown.</param>
    public void DecrementCooldown(float timeToReduce)
    {
        _parry.SetCooldown(_parry.Cooldown - timeToReduce);
    }

    /// <summary>
    /// Saves the current player statistics to a temporary storage object and triggers the statistics changed event.
    /// </summary>
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

    /// <summary>
    /// Applies damage to the entity, updates health, and triggers parry or death logic as appropriate.
    /// </summary>
    /// <param name="damage">Amount of damage to apply.</param>
    /// <param name="combat">Combat context associated with the damage event.</param>
    public void TakeDamage(float damage, Combat combat)
    {
        if (_isDead || _isInvulnerable) return;

        if (_parry != null && _parry.IsParrying)
        {
            _parry.SucessfulParry(combat);
            return;
        }
        
        //CHEATS
        if (_godMode) return;

        _health -= damage;
        DispatchHealthChanged();

        CancelCurrentAction();
        animator.SetTrigger("TakingDamage");

        if (_health <= 0)
        {
            _health = 0;
            Die();
        }
        else
        {
            StartCoroutine(InvulnerabilityRoutine());
        }
    }

    private void CancelCurrentAction()
    {
        animator.ResetTrigger("Attack");
        animator.SetFloat("Speed", 0f);
    }

    private IEnumerator InvulnerabilityRoutine()
    {
        _isInvulnerable = true;
        yield return new WaitForSeconds(invulnerabilityDuration);
        _isInvulnerable = false;
    }

    private void Die()
    {
        if (_isDead) return;
        _isDead = true;

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        if (_playerMovement != null) _playerMovement.enabled = false;
        if (_playerCombat != null) _playerCombat.enabled = false;
        if (_parry != null) _parry.enabled = false;

        PlayerRotation rotation = GetComponent<PlayerRotation>();
        if (rotation != null) rotation.enabled = false;

        animator.SetTrigger(triggerDie);
        StartCoroutine(DisableAfterDeath());
    }

    private IEnumerator DisableAfterDeath()
    {
        yield return null;

        float length = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(length);

        gameObject.SetActive(false);

        SaveManager save = FindFirstObjectByType<SaveManager>();
        if (save != null && save.HasSavedGame())
            save.DeleteSave();

        if (deathScreenUI != null)
            deathScreenUI.SetActive(true);
        else
            Debug.LogError("PlayerStats: deathScreenUI não está assignado no Inspector!");
    }

    /// <summary>
    /// Retrieves the unique identifier used for saving player statistics.
    /// </summary>
    /// <returns>The save ID string for player statistics.</returns>
    public string GetSaveID()
    {
        return "PlayerStats";
    }

    /// <summary>
    /// Serializes the current player's save data to a JSON string.
    /// </summary>
    /// <returns>A JSON string representation of the player's save data.</returns>
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
        };
        return JsonUtility.ToJson(data);
    }

    /// <summary>
    /// Loads player stats from a JSON string and updates the player's attributes accordingly.
    /// </summary>
    /// <param name="json">The JSON string containing the player stats.</param>
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

    /// <summary>
    /// Represents the data required to save and restore player state, including health, movement, attack, shield, stun,
    /// knockback, and buff parameters.
    /// </summary>
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
    }
}
