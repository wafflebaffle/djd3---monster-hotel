using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStats : MonoBehaviour, IDamageable, IParryable
{
    [SerializeField] private StatsData stats;
    [SerializeField] private EnemyEventChannel events;
    [SerializeField] private string takeDamageAnimName = "TakeDamage";
    [SerializeField] private string deathAnimName = "Death";
    [SerializeField] private float deathAnimTime = 0.5f;

    private Renderer[] _renderers;
    private Color[] _originalColors;

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

        _renderers = GetComponentsInChildren<Renderer>();
        _originalColors = new Color[_renderers.Length];
        for (int i = 0; i < _renderers.Length; i++)
        {
            _originalColors[i] = _renderers[i].material.color;
        }
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


    public void OnParried(Vector3 direction)
    {
        StartCoroutine(ParryEffect(direction));
    }

    private IEnumerator ParryEffect(Vector3 direction)
    {
        float stunTime = 0.5f; //mudar para algo incrementavel e serializavel
        float knockbackDistance = 5f; //mudar para algo incrementavel e serializavel

        NavMeshAgent agent = GetComponent<NavMeshAgent>();

        SetColor(Color.blue);

        if (agent != null)
        {
            agent.isStopped = true;
        }


        Vector3 targetPos = transform.position + direction * knockbackDistance;

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * 6f;
            transform.position = Vector3.Lerp(transform.position, targetPos, t);
            yield return null;
        }

        yield return new WaitForSeconds(stunTime);

        ResetColor();

        if (agent != null)
        {
            agent.isStopped = false;
        }
    }
}