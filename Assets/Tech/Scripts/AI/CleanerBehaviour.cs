using System;
using LibGameAI.FSMs;
using UnityEngine;

public class CleanerBehaviour : AIBehaviour
{
    private EnemyMovement _movement;
    private EnemyCombat _combat;
    private EnemySight _sight;
    private EnemyStats _stats;
    private StateMachine _fsm;
    private State _idle;
    private State _chase;
    private State _attack;
    private State _stun;

    protected override void Start()
    {
        _movement = GetComponent<EnemyMovement>();
        _combat = GetComponent<EnemyCombat>();
        _sight = GetComponent<EnemySight>();
        _stats = GetComponent<EnemyStats>();

        _idle = new State("Idle", null, _movement.MoveRandom, null);
        _chase = new State("Chase", null, _movement.Move, null);
        _attack = new State("Attack", _combat.DoAttack, null, null);
        _stun = new State("Stun", null, null, () => _sight.GetTarget(_sight.Target));

        Transition idleToChaseBySight = new Transition(() => _sight.GetTarget(), null, _chase);
        _idle.AddTransition(idleToChaseBySight);
        Transition chaseToAttack = new Transition(_combat.CanAttack, null, _attack);
        _chase.AddTransition(chaseToAttack);
        Transition chaseToIdle = new Transition(() => _sight.GetTarget() == false, null, _idle);
        _chase.AddTransition(chaseToIdle);
        Transition attackToIdle = new Transition(() => _combat.HadAttack, null, _idle);
        _attack.AddTransition(attackToIdle);
        Transition anyToStun = new Transition(() => _stats.IsStunned, _movement.StopMove, _stun);
        _idle.AddTransition(anyToStun);
        _chase.AddTransition(anyToStun);
        _attack.AddTransition(anyToStun);
        Transition stunToChase = new Transition(() => _stats.IsStunned == false, _movement.ReableMove, _chase);
        _stun.AddTransition(stunToChase);

        _fsm = new StateMachine(_idle);
    }

    protected override void Update()
    {
        _fsm.Update()?.Invoke();
    }
}