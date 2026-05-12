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
        if (_enemy.IsStunned)
        {
            _agent.ResetPath();
            return;
        }

        Move();  
    }

    public void SetSpeed(float value)
    {
        Speed = value;
    }

    public void SetAngularSpeed(float value)
    {
        AngularSpeed = value;
    }

    private void Move()
    {
        _currentTarget = _enemy.GetTarget();
        Vector3 target;

        if (_currentTarget)
        {
            _agent.speed = Speed*pursuitVelocityMultiplier;
            target = _currentTarget.position;
        } 
        else if(_lastTarget == null || _hasArrive || _lastPos == transform.position)
        {
            _agent.speed = Speed;
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
}
