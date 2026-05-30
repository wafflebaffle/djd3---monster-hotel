using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStats : MonoBehaviour, IDamageable, IParryable
{
    [SerializeField] private StatsData stats;
    [SerializeField] private EnemyEventChannel events;
    [SerializeField] private Animator enemyAnim;
    [SerializeField] private string takeDamageAnimName = "TakeDamage";
    [SerializeField] private string deathAnimName = "Death";
    [SerializeField] private float deathAnimTime = 0.5f;
    public Animator Animator => enemyAnim;

    private Renderer[] _renderers;
    private Color[] _originalColors;

    private float _health;
    private EnemyMovement _enemyMovement;
    private EnemySight _enemySight;
    private Combat _enemyCombat;

    private bool _isStunned;
    public bool IsStunned => _isStunned;

    //Propriedades
    public float CurrentHealth => _health;
    public float MaxHealth => stats.maxHealth;
    public float AttackDamage => stats.attackDamage;
    public float AttackRange => stats.attackRange;
    public float ShieldCooldown => stats.shieldCooldown;
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
        events.RaiseEnemySpawned();

        _enemyMovement = GetComponent<EnemyMovement>();
        _enemySight = GetComponent<EnemySight>();
        _enemyMovement.SetSpeed(stats.moveSpeed);
        _enemyMovement.SetAngularSpeed(stats.angularSpeed);
        _enemyCombat = GetComponent<Combat>();
        _enemyCombat.SetDamage(stats.attackDamage);
        _enemyCombat.SetRange(stats.attackRange);
        _enemyCombat.SetCooldown(stats.attackCooldown);
        _health = stats.maxHealth;

        _renderers = GetComponentsInChildren<Renderer>();
        _originalColors = new Color[_renderers.Length];
        for (int i = 0; i < _renderers.Length; i++)
        {
            _originalColors[i] = _renderers[i].material.color;
        }
    }

    public void TakeDamage(float damage, Combat combat)
    {
        if (combat is EnemyCombat or RangedEnemyCombat) return;

        _health -= damage;
        enemyAnim.SetTrigger(takeDamageAnimName);
        _enemySight.SetTarget(combat.transform);
        
        DispatchHealthChanged();
        if(_health <= 0)
        {
            _health = 0;
            Death();
        }
    }
    public void Death()
    {
        //Play death animation;
        enemyAnim.SetTrigger(deathAnimName);

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

    private void SetColor(Color color)
    {
        for (int i = 0; i < _renderers.Length; i++)
        {
            _renderers[i].material.color = color;
        }
    }

    private void ResetColor()
    {
        for (int i = 0; i < _renderers.Length; i++)
        {
            _renderers[i].material.color = _originalColors[i];
        }
    }
    
    public void ParryEffect(Vector3 direction, float stunTime = 0.5f, float knockbackDistance = 0.0f)
    {
        ApplyStun();
        SetColor(Color.blue);

        Vector3 targetPos = transform.position + direction * knockbackDistance;

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * 6f;
            transform.position = Vector3.Lerp(transform.position, targetPos, t);
        }

        Invoke("RemoveStun", stunTime);
    }

    public void ApplyStun()
    {
        _isStunned = true;
    }

    public void RemoveStun()
    {
        _isStunned = false;
        ResetColor();
    }
}