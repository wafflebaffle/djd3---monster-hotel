using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private string moveInput = "Move";
    public float Speed { get; private set; }
    private Vector2 _direction;
    private CharacterController _controller;
    private InputAction _move;
    private Camera _cam;
    private PlayerStats _stats;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _stats = GetComponent<PlayerStats>();
        _cam = Camera.main;
        _move = InputSystem.actions.FindAction(moveInput);
    }

    private void Update()
    {
        Vector3 motion = HandleMovement();
        motion += HandleGravity();

        _controller.Move(motion);
    }

    private Vector3 HandleMovement()
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
        motion *= Speed*Time.deltaTime;

        _stats.Animator.SetFloat("Velocity", motion.magnitude);

        if (Time.timeScale != 0) return motion;
        else return Vector3.zero;
    }

    private Vector3 HandleGravity()
    {
        return Vector3.down * 9.81f * Time.deltaTime;
    }

    public void SetSpeed(float value)
    {
        Speed = value;
    }
}
