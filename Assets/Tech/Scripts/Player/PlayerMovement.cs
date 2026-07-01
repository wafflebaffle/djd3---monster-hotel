using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private string moveInput = "Move";
    public float Speed { get; private set; }

    private Vector2 _direction;
    private CharacterController _controller;
    private InputAction _move;
    private PlayerStats _stats;
    private PlayerCombat _combat;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _stats = GetComponent<PlayerStats>();
        _combat = GetComponent<PlayerCombat>();
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
        Vector3 worldMoveDir = new Vector3(_direction.x, 0, _direction.y).normalized;
        Vector3 localDir = transform.InverseTransformDirection(worldMoveDir);
        bool isAttacking = _combat != null && _combat.IsAttacking;

        Vector3 motion = Vector3.zero;

        if (!isAttacking)
        {
            motion = worldMoveDir * Speed * Time.deltaTime;

            if (_direction.y > 0)
                motion += Vector3.forward;
            else if (_direction.y < 0)
                motion += -Vector3.forward;
            if (_direction.x > 0)
                motion += Vector3.right;
            else if (_direction.x < 0)
                motion += -Vector3.right;

            motion = motion.normalized;
            motion *= Speed * Time.deltaTime;
        }

        if (_stats.Animator != null)
        {
            _stats.Animator.SetFloat("VelocityX", isAttacking ? 0 : localDir.x, 0.1f, Time.deltaTime);
            _stats.Animator.SetFloat("VelocityZ", isAttacking ? 0 : localDir.z, 0.1f, Time.deltaTime);
        }

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
