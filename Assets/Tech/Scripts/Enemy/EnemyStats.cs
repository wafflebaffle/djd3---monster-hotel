using System;
using System.Collections;
using UnityEngine;

public class EnemyStats : MonoBehaviour, IDamageable, IParryable
{
    [SerializeField] private StatsData stats;
    [SerializeField] private EnemyEventChannel events;
    [SerializeField] private string takeDamageAnimName = "TakeDamage";
    [SerializeField] private string deathAnimName = "Death";
    [SerializeField] private float deathAnimTime = 0.5f;
    private float _health;
    private EnemyMovement _enemyMovement;
    private EnemySight _enemySight;
    private EnemyCombat _enemyCombat;
    private Animator _enemyAnim;

    //Propriedades
    public float CurrentHealth => _health;
    public float MaxHealth => stats.maxHealth;
    public float AttackDamage => stats.attackDamage;
    public float CooldownReduction => stats.cooldownReduction;
    public float Speed => stats.moveSpeed;
    public float AngularSpeed => stats.angularSpeed;
    
    //Eventos
    public event Action OnHealthChanged;

    private void DispatchHealthChanged()
    {
        OnHealthChanged?.Invoke();
    }

    private void Start()
    {
        _enemyAnim = GetComponent<Animator>();
        events.RaiseEnemySpawned();

        _enemyMovement = GetComponent<EnemyMovement>();
        _enemySight = GetComponent<EnemySight>();
        _enemyMovement.SetSpeed(stats.moveSpeed);
        _enemyMovement.SetAngularSpeed(stats.angularSpeed);
        _enemyCombat = GetComponent<EnemyCombat>();
        _enemyCombat.SetDamage(stats.attackDamage);
        _enemyCombat.SetRange(stats.attackRange);
        _enemyCombat.SetCooldown(stats.attackCooldown);
        _health = stats.maxHealth;
    }

    public void TakeDamage(float damage, Combat combat)
    {
        if (combat is EnemyCombat) return;

        _health -= damage;
        _enemyAnim.SetTrigger(takeDamageAnimName);
        _enemySight.SetTarget(combat.transform);
        
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
        _enemyAnim.SetTrigger(deathAnimName);

        StartCoroutine(Died());
    }

    private IEnumerator Died()
    {
        YieldInstruction wfs = new WaitForSeconds(deathAnimTime);

        yield return wfs;

        events.RaiseEnemyDied();
        gameObject.SetActive(false);
    }

    public Transform GetTarget()
    {
        return _enemySight.Target;
    }

    public float DistanceToTarget()
    { return _enemyMovement.DistanceToTarget(); }

    public void OnParried(Vector3 direction)
    {
        StartCoroutine(ParryEffect(direction));
    }

    private IEnumerator ParryEffect(Vector3 direction)
    {
        float stunTime = 0.5f; //mudar para algo incrementavel e serializavel
        float knockbackForce = 5f; //mudar para algo incrementavel e serializavel

        EnemyMovement movement = GetComponent<EnemyMovement>();
        Rigidbody rb = GetComponent<Rigidbody>();

        if (movement != null)
        {
            movement.enabled = false;
        }


        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.AddForce(direction * knockbackForce, ForceMode.Impulse);
        }

        yield return new WaitForSeconds(stunTime);

        if (movement != null)
        {
            movement.enabled = true;
        }
    }
}