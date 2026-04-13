using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class EnemyMovement : MonoBehaviour
{
    public float Speed { get; private set; }
    public float AngularSpeed { get; private set; }
    private CharacterController _controller;
    private EnemyStats _enemy;
    private Transform _currentTarget;
    private Func<Transform, SteeringOutput> _steer;

    private void Start()
    {
        _enemy = GetComponent<EnemyStats>();
    }
    private void Update()
    {
        
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
            // Obtain steering (velocity and angular velocity) given a target
            SteeringOutput steering = _steer(_currentTarget);

            _controller.SimpleMove(transform.position + steering.Linear * Time.deltaTime);
            transform.Rotate(0,transform.rotation.eulerAngles.y + steering.Angular * Time.deltaTime,0);
        }
    }

    private SteeringOutput GetSeekSteering(Transform target)
        {
            // Initialize linear and angular velocity to zero
            Vector2 linear = Vector2.zero;
            float angular = 0f;

            // Do I have a target?
            if (target != null)
            {
                // Get the direction to the target
                linear = target.position - transform.position;

                // The velocity is along this direction, at full speed
                linear = linear.normalized * Speed;

                // Face in the direction we want to move
                transform.eulerAngles =
                    new Vector3(0, GetNewOrientation(transform.rotation.eulerAngles.y, linear), 0);

                // Angular velocity not used here, we already changed orientation
                // in the code above
                angular = 0;
            }

            // Output the steering
            return new SteeringOutput(linear, angular);
        }

        // This function is called by the steering behaviours in order to determine
        // a new orientation based on the current orientation and velocity
        private float GetNewOrientation(float orientation, Vector3 velocity)
        {
            // Make sure we have a velocity
            if (velocity.magnitude > 0)
            {
                // Calculate orientation using an arc tangent of the velocity
                // components
                return Mathf.Atan2(velocity.x, velocity.z) * Mathf.Rad2Deg;
            }
            else
            {
                // Otherwise use the current orientation
                return orientation;
            }
        }

        // Wander behaviour
        private SteeringOutput GetWanderSteering(Transform target)
        {
            // Get vector form of the current rotation of this game object
            Vector2 orientation = new Vector2(
                Mathf.Cos(transform.rotation.eulerAngles.y * Mathf.Deg2Rad),
                Mathf.Sin(transform.rotation.eulerAngles.y * Mathf.Deg2Rad));

            // Get velocity from the vector form of the orientation
            Vector2 linear = Speed * orientation;

            // Change our orientation randomly
            float angular = (Random.Range(0, 1f) - Random.Range(0, 1f))
                * AngularSpeed;

            // Output the steering
            return new SteeringOutput(linear, angular);
        }
}
