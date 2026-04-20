using UnityEngine;
using System;
using Random = UnityEngine.Random;
using UnityEngine.UIElements;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private RoomArea room;
    public float Speed { get; private set; }
    public float AngularSpeed { get; private set; }
    private CharacterController _controller;
    private EnemyStats _enemy;
    private Transform _currentTarget;
    private Func<Transform, SteeringOutput> _steer;

    private void Start()
    {
        _enemy = GetComponent<EnemyStats>();
        _controller = GetComponent<CharacterController>();
    }
    private void FixedUpdate()
    {
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

        if (_currentTarget)
        {
            _steer = GetSeekSteering;
            // Obtain steering (velocity and angular velocity) given a target
            SteeringOutput steering = _steer(_currentTarget);

            _controller.SimpleMove(steering.Linear);
            transform.Rotate(0,steering.Angular * Time.deltaTime,0);
        }
        else
        {
            //_currentTarget = gameObject.transform;
            _steer = GetWanderSteering;

            // Obtain steering (velocity and angular velocity) given a target
            SteeringOutput steering = _steer(_currentTarget);

            _controller.SimpleMove(steering.Linear);
            transform.Rotate(0,steering.Angular * Time.deltaTime,0);
        }
    }

    private SteeringOutput GetSeekSteering(Transform target)
        {
            // Initialize linear and angular velocity to zero
            Vector3 linear = Vector2.zero;
            float angular = 0f;

            // Do I have a target?
            if (target)
            {
                // Get the direction to the target
                linear = target.position - transform.position;

                // The velocity is along this direction, at full speed
                linear = linear.normalized * Speed;

                // Face in the direction we want to move
                float desiredOrientation = Mathf.Atan2(linear.x, linear.z) * Mathf.Rad2Deg;
                float currentOrientation = transform.rotation.eulerAngles.y;

                float angleDifference = Mathf.DeltaAngle(currentOrientation, desiredOrientation);
                angular = Mathf.Clamp(angleDifference, -AngularSpeed, AngularSpeed);
            }

            // Output the steering
            return new SteeringOutput(linear, angular);
        }

        // Wander behaviour
        private SteeringOutput GetWanderSteering(Transform target)
        {
            // Get vector form of the current rotation of this game object
            Vector3 linear = transform.forward * Speed;

            // Change our orientation randomly
            float angular = (Random.Range(0, 1f) - Random.Range(0, 1f))
                * AngularSpeed;

            // Output the steering
            return new SteeringOutput(linear, angular);
        }
}
