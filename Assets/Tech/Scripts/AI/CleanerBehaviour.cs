using LibGameAI.FSMs;

public class CleanerBehaviour : AIBehaviour
{
    private EnemyMovement _movement;
    private EnemyCombat _combat;
    private EnemySight _sight;

    protected override void Start()
    {
        _movement.GetComponent<EnemyMovement>();
        _combat.GetComponent<EnemyCombat>();
        _sight.GetComponent<EnemySight>();

        State idle = new State("Idle", null , _movement.MoveRandom, null);
        State chase = new State("Chase", null, _movement.Move, null);
        State attack = new State("Attack", _combat.DoAttack, null, null);

        Transition idleToChase = new Transition(() => _sight.Target, null, chase);
        Transition chaseToAttack = new Transition(_combat.CanAttack, null, attack);
        Transition attackToIdle = new Transition(() => _combat.HasAttack, _combat.IdkIJustWantToTurnThisOff, idle);

        StateMachine fsm = new StateMachine(idle);
    }

    protected override void Update()
    {
        
    }
}