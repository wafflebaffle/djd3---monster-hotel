using System;
using LibGameAI.FSMs;
using UnityEngine;

public class WaiterShooterBehaviour : AIBehaviour
{
    [SerializeField] private float fleeDistance;
    private EnemyMovement _movement;
    private RangedEnemyCombat _combat;
    private EnemySight _sight;
    private StateMachine _fsm;
    private State _idle;
    private State _chase;
    private State _attack;
    private State _flee;

    protected override void Start()
    {
        _movement = GetComponent<EnemyMovement>();
        _combat = GetComponent<RangedEnemyCombat>();
        _sight = GetComponent<EnemySight>();

        _idle = new State("Idle", null, _movement.MoveRandom, null);
        _chase = new State("Chase", _movement.Move, _movement.FocusTarget, null);
        _attack = new State("Attack", _combat.DoAttack, null, null);
        _flee = new State("Flee", null, _movement.Flee, null);

        Transition idleToChase = new Transition(() => _sight.GetTarget(), null, _chase);
        _idle.AddTransition(idleToChase);
        Transition chaseToAttack = new Transition(_combat.CanAttack, null, _attack);
        _chase.AddTransition(chaseToAttack);
        Transition chaseToIdle = new Transition(() => _sight.GetTarget() == false, null, _idle);
        _chase.AddTransition(chaseToIdle);
        Transition attackToIdle = new Transition(() => _combat.HadAttack, null, _idle);
        _attack.AddTransition(attackToIdle);
        Transition chaseToFlee = new Transition(() => _movement.DistanceToTarget() < fleeDistance, null, _flee);
        _chase.AddTransition(chaseToFlee);
        Transition fleeToChase = new Transition(() => _movement.DistanceToTarget() > fleeDistance, null, _chase);
        _flee.AddTransition(fleeToChase);

        _fsm = new StateMachine(_idle);
    }

    protected override void Update()
    {
        _fsm?.Update()?.Invoke();
    }
}