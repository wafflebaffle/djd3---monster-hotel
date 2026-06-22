using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float marginToMove = 0.9f;
    [SerializeField] private float pursuitVelocityMultiplier = 1.5f;
    private RoomArea _room;
    public float Speed { get; private set; }
    public float AngularSpeed { get; private set; }
    private EnemyStats _enemy;
    private NavMeshAgent _agent;
    private Transform _currentTarget;
    private bool _hasArrive => Vector3.Distance(transform.position, _agent.destination) <= _agent.stoppingDistance;
    private Vector3? _lastTarget;
    private Vector3 _lastPos;

    private void Start()
    {
        _enemy = GetComponent<EnemyStats>();
        _agent = GetComponent<NavMeshAgent>();
        _room = GetComponentInParent<RoomArea>();

        _agent.speed = Speed;
        _agent.angularSpeed = AngularSpeed;

        _lastTarget = null;
    }

    private void Update()
    {
        if (_agent.isStopped) return;
        _enemy.Animator.SetFloat("Velocity", _agent.velocity.magnitude);
    }

    public void SetSpeed(float value)
    {
        Speed = value;
    }

    public void SetAngularSpeed(float value)
    {
        AngularSpeed = value;
    }

    public void StopMove()
    {
        _agent.isStopped = true;
    }

    public void ReableMove()
    {
        _agent.isStopped = false;
    }

    public void Move()
    {
        _agent.speed = Speed;

        _currentTarget = _enemy.GetTarget();
        Vector3 target;

        _agent.speed = Speed*pursuitVelocityMultiplier;
        target = _currentTarget.position;

        _lastPos = transform.position;
        _agent.destination = target;
    }

    public void MoveWithDistance()
    {
        _agent.speed = Speed * pursuitVelocityMultiplier;

        _currentTarget = _enemy.GetTarget();
        Vector3 dir = (_lastTarget.Value - transform.position).normalized;
        _agent.destination = transform.position + dir * _enemy.AttackRange;

        _lastPos = transform.position;
    }

    public void MoveRandom()
    {
        _agent.speed = Speed;

        Vector3 target;

        if(_lastTarget == null || _hasArrive || isStopped())
        {
            target = _room.RandomPosition(marginToMove);
            _lastTarget = target;
        }
        else target = _lastTarget.Value;

        _lastPos = transform.position;
        _agent.destination = target;
    }

    public float DistanceToTarget()
    {
        if(_currentTarget) return Vector3.Distance(transform.position, _currentTarget.position);
        return float.NaN;
    }

    public void Flee()
    {
        _agent.speed = Speed * pursuitVelocityMultiplier;
        Vector3 dir = (_lastTarget.Value - transform.position).normalized;
        _lastPos = transform.position;
        _agent.destination = _lastTarget.Value - dir * _enemy.AttackRange;
    }

    public void RotateToTarget()
    {
        Vector3 target = _enemy.GetTarget() ? _enemy.GetTarget().position : _lastTarget.Value;

        Vector3 dir = (target - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, AngularSpeed * Time.deltaTime);
    }

    public bool isStopped()
    {
        return _lastPos == transform.position;
    }
}
