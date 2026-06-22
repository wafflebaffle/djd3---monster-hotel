using System;
using System.Collections;
using UnityEngine;

public class EnemyStats : MonoBehaviour, IDamageable, IParryable
{
    [SerializeField] private StatsData stats;
    [SerializeField] private EnemyEventChannel events;
    [SerializeField] private Animator enemyAnim;
    [SerializeField] private string takeDamageAnimName = "TakeDamage";
    [SerializeField] private string deathAnimName = "Death";
    [SerializeField] private float deathAnimTime = 0.5f;
    [SerializeField] private float stunDuration = 0.4f;
    public Animator Animator => enemyAnim;
    private Vector3 _startPos = new();

    private Renderer[] _renderers;
    private Color[] _originalColors;

    private float _health;
    private EnemyMovement _enemyMovement;
    private EnemySight _enemySight;
    private Combat _enemyCombat;

    private bool _isStunned;
    public bool IsStunned => _isStunned;
    private bool _isDead;
    public bool IsDead => _isDead;

    private AIBehaviour _aiBehaviour;

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

    private void OnDisable()
    {
        transform.position = _startPos;
    }

    private void Awake()
    {
        events.RaiseEnemySpawned();
        _startPos = transform.position;

        _enemyMovement = GetComponent<EnemyMovement>();
        _enemySight = GetComponent<EnemySight>();
        _aiBehaviour = GetComponent<AIBehaviour>();
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
        if (_isDead) return;
        if (combat is EnemyCombat or RangedEnemyCombat) return;

        _health -= damage;
        _enemySight.SetTarget(combat.transform);

        if (_health <= 0)
        {
            _health = 0;
            Death();
            return;
        }

        enemyAnim.SetTrigger(takeDamageAnimName);
        ApplyStun(stunDuration);
        DispatchHealthChanged();
    }
    public void Death()
    {
        Debug.Log("Death() foi chamado!");
        if (_isDead) return;
        _isDead = true;

        _enemyMovement.StopMove();
        if (_aiBehaviour) _aiBehaviour.enabled = false;

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
        if (_isDead) return;

        ApplyStun(stunTime);
        SetColor(Color.blue);

        Vector3 targetPos = transform.position + direction * knockbackDistance;

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * 6f;
            transform.position = Vector3.Lerp(transform.position, targetPos, t);
        }
    }

    public void ApplyStun(float stunTime)
    {
        if (_isDead) return;

        CancelInvoke(nameof(RemoveStun));

        _isStunned = true;
        _enemyCombat.CancelAttack();

        Invoke(nameof(RemoveStun), stunTime);
    }

    public void RemoveStun()
    {
        _isStunned = false;
        ResetColor();
    }
}