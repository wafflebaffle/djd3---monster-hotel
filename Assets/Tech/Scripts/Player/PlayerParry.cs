using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerParry : MonoBehaviour
{
    [SerializeField] private float parryOp = 0.3f;
    [SerializeField] private float cooldown = 1f;
    [SerializeField] private string Input = "Parry";

    private InputAction _parryAction;
    private bool _isParrying;

    public bool IsParrying => _isParrying;

    private float Timer;
    private float _lastParried;

    private void Start()
    {
        _parryAction = InputSystem.actions.FindAction(Input);
    }

    private void Update()
    {
        HandleInput();
        HandleParryOpening();
    }


    private void HandleInput()
    {
        if (_parryAction == null) return;

        if (_parryAction.WasPressedThisFrame())
        {
            TryParry();
        }
    }


    private void HandleParryOpening()
    {
        if (!_isParrying)
        {
            return;
        }

        Timer += Time.deltaTime;

        if (Timer >= parryOp)
        {
            _isParrying = false;
        }
    }

    private void TryParry()
    {
        if (Time.time < _lastParried + cooldown) return;

        _lastParried = Time.time;
        _isParrying = true;
        Timer = 0f;

        //som do parry
    }

    public void SucessfulParry(Combat enemy)
    {
        _isParrying = false;

        if (enemy == null) return;

        Vector3 direction = (enemy.transform.position - transform.position).normalized;

        if (enemy.TryGetComponent<IParryable>(out IParryable parryable))
        {
            parryable.OnParried(direction);
        }
    }

}
