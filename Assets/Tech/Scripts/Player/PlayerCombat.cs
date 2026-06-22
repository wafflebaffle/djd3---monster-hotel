using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class PlayerCombat : Combat
{
    private InputAction _attack;
    private IDamageable _stats;

    public float AttackCooldown => attackCooldown;

    [Header("References")]
    [SerializeField] private string attackInput = "Attack";
    [SerializeField] private Animator attackAnim;
    [SerializeField] private string attackAnimName = "Punch";
    [SerializeField] private float attackAnimDuration = 1.0f;

    [Header("Combo")]
    [SerializeField] private string hit1BoolName = "hit1";
    [SerializeField] private string hit2BoolName = "hit2";
    [SerializeField] private float comboResetTime = 1.0f;

    private bool isFirstPunch = true;
    private float lastPunchTime;

    [Header("Audio")]
    [SerializeField] private AudioMixerGroup sfxGroup;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip hitSound;

    void Start()
    {
        _attack = InputSystem.actions.FindAction(attackInput);
        _stats = GetComponent<PlayerStats>();
        SetCooldown(attackAnimDuration);

        if (audioSource && sfxGroup)
        {
            audioSource.outputAudioMixerGroup = sfxGroup;
        }
    }
    void Update()
    {
        if (!isFirstPunch && Time.time - lastPunchTime > comboResetTime)
        {
            isFirstPunch = true;
        }

        if (_attack.WasPressedThisFrame())
        {
            TryAttack();
        }
    }

    protected override void TryAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown)
            return;

        lastAttackTime = Time.time;
        lastPunchTime = Time.time;

        attackAnim.SetBool(hit1BoolName, isFirstPunch);
        attackAnim.SetBool(hit2BoolName, !isFirstPunch);
        attackAnim.SetTrigger(attackAnimName);

        isFirstPunch = !isFirstPunch;

        if (audioSource && attackSound)
        {
            audioSource.pitch = Random.Range(0.95f, 1.05f);
            audioSource.PlayOneShot(attackSound);
        }
        StartCoroutine(Attack());
        StartCoroutine(ResetPunchBoolsAfterAnim());
    }

    private IEnumerator ResetPunchBoolsAfterAnim()
    {
        yield return new WaitForSeconds(attackAnimDuration);
        attackAnim.SetBool(hit1BoolName, false);
        attackAnim.SetBool(hit2BoolName, false);
    }

    protected override IEnumerator Attack()
    {
        YieldInstruction wfs = new WaitForSeconds(attackAnimDuration);

        yield return wfs;

        Vector3 directionToTarget;
        Vector3 closestTargetPosition = Vector3.zero;
        IDamageable closestTarget = null;
        float closestDistance = Mathf.Infinity;

        
        Collider[] hits = Physics.OverlapSphere(
            attackPoint.position,
            attackRange
        );

        foreach (Collider hit in hits)
        {
            if (hit.TryGetComponent<IDamageable>(out IDamageable damageableTarget))
            {  
                if(damageableTarget == _stats) continue;

                float distance = Vector3.Distance(hit.transform.position, transform.position);
        
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = damageableTarget;
                    closestTargetPosition = hit.transform.position;
                }
            }
        }

        if (closestTarget != null)
        {
            directionToTarget = (closestTargetPosition - transform.position).normalized;
            float dot = Vector3.Dot(transform.forward, directionToTarget);

            if (dot > 0.5f) // (0.5 ≈ 60)
            {
                closestTarget.TakeDamage(attackDamage, this);
                if (closestTarget is MonoBehaviour mb)
                {
                    if (mb.TryGetComponent<EnemyStats>(out var enemy))
                    {
                        enemy.ParryEffect(directionToTarget);
                    }
                }
                if (audioSource && hitSound)
                {
                    audioSource.pitch = Random.Range(0.95f, 1.05f);
                    audioSource.PlayOneShot(hitSound);
                }
                    
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}