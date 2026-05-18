using System;
using LibGameAI.FSMs;
using UnityEngine;

public class CleanerBehaviour : AIBehaviour
{
    private EnemyMovement _movement;
    private EnemyCombat _combat;
    private EnemySight _sight;
    private StateMachine _fsm;
    private State _idle;
    private State _chase;
    private State _attack;

    protected override void Start()
    {
        _movement = GetComponent<EnemyMovement>();
        _combat = GetComponent<EnemyCombat>();
        _sight = GetComponent<EnemySight>();

        _idle = new State("Idle", null, _movement.MoveRandom, null);
        _chase = new State("Chase", null, _movement.Move, null);
        _attack = new State("Attack", _combat.DoAttack, null, null);

        Transition idleToChase = new Transition(_sight.GetTarget, null, _chase);
        _idle.AddTransition(idleToChase);
        Transition chaseToAttack = new Transition(_combat.CanAttack, null, _attack);
        _chase.AddTransition(chaseToAttack);
        Transition chaseToIdle = new Transition(() => _sight.GetTarget() == false, null, _idle);
        _chase.AddTransition(chaseToIdle);
        Transition attackToIdle = new Transition(() => {Debug.Log($"HadAttack value: {_combat.HadAttack}"); return _combat.HadAttack; }, null, _idle);
        _attack.AddTransition(attackToIdle);

        _fsm = new StateMachine(_idle);
    }

    protected override void Update()
    {
        _fsm.Update()?.Invoke();
    }
}