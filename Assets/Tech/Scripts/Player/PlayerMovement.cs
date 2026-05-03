using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private string moveInput = "Move";
    public float Speed { get; private set; }
    private Vector2 _direction;
    private CharacterController _controller;
    private InputAction _move;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();

        _move = InputSystem.actions.FindAction(moveInput);
    }

    private void Update()
    {
        HandleMovement();
        //HandleGravity();
    }

    private void HandleMovement()
    {
        _direction = _move.ReadValue<Vector2>();
        Vector3 motion = Vector3.zero;
        
        if (_direction.y > 0)
            motion += Vector3.forward;
        else if (_direction.y < 0)
            motion += -Vector3.forward;

        if (_direction.x > 0)
            motion += Vector3.right;
        else if (_direction.x < 0)
            motion += -Vector3.right;

        motion = motion.normalized;
        motion *= Speed*Time.fixedDeltaTime;

        if (Time.timeScale != 0) _controller.Move(motion);
    }

    public void SetSpeed(float value)
    {
        Speed = value;
    }
}
