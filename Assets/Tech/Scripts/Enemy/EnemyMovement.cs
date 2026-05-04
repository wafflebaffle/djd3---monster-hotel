using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float timerPerDecision = 1.0f;
    [SerializeField] private float marginToMove = 0.9f;
    [SerializeField] private float pursuitVelocityMultiplier = 1.5f;
    private float _timer;
    private RoomArea _room;
    public float Speed { get; private set; }
    public float AngularSpeed { get; private set; }
    private EnemyStats _enemy;
    private NavMeshAgent _agent;
    private Transform _currentTarget;

    private void Start()
    {
        _enemy = GetComponent<EnemyStats>();
        _agent = GetComponent<NavMeshAgent>();
        _room = GetComponentInParent<RoomArea>();

        _agent.speed = Speed;
        _agent.angularSpeed = AngularSpeed;
    }
    private void FixedUpdate()
    {
        if (_enemy.IsStunned)
        {
            _agent.ResetPath();
            return;
        }

        _timer += Time.deltaTime;

        if(_timer >= timerPerDecision)
        {
            Move();
            _timer = 0;
        }  
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

        if (_currentTarget)
        {
            _agent.speed = Speed*pursuitVelocityMultiplier;
            _agent.destination = _currentTarget.position;
        }
        else
        {
            _agent.speed = Speed;
            _agent.destination = _room.RandomPosition(marginToMove);
        }
    }

    public float DistanceToTarget()
    {
        if(_currentTarget) return Vector3.Distance(transform.position, _currentTarget.position);
        return float.NaN;
    }
}
