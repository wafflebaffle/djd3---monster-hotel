using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private string moveInput = "Move";
    
    private float _velocity;
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
        Vector3 direction3D = new Vector3(_direction.x,0,_direction.y);
        
        if (_direction.x > 0)
            _controller.Move(transform.right*speed*Time.fixedDeltaTime);
        else if (_direction.x < 0)
            _controller.Move(-transform.right*speed*Time.fixedDeltaTime);

        if (_direction.y > 0)
            _controller.Move(transform.forward*speed*Time.fixedDeltaTime);
        else if (_direction.y < 0)
            _controller.Move(-transform.forward*speed*Time.fixedDeltaTime);
    }

    private void HandleGravity()
    {
        _controller.Move(Vector3.up*gravity*Time.fixedDeltaTime);
    }
}
