using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private string moveInput = "Move";
    private Vector2 _direction;
    private CharacterController _controller;
    private InputAction _move;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();

        _move = InputSystem.actions.FindAction(moveInput);
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleGravity();
    }

    private void HandleMovement()
    {
        _direction = _move.ReadValue<Vector2>();
        Vector3 motion = Vector3.zero;
        
        if (_direction.y > 0)
            motion += transform.forward;
        else if (_direction.y < 0)
            motion += -transform.forward;

        if (_direction.x > 0)
            motion += transform.right;
        else if (_direction.x < 0)
            motion += -transform.right;

        motion = motion.normalized;
        motion *= speed*Time.fixedDeltaTime;

        _controller.Move(motion);
    }

    private void HandleGravity()
    {
        _controller.Move(Vector3.up*gravity*Time.fixedDeltaTime);
    }
}
