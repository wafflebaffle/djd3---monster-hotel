using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerParry : MonoBehaviour
{
    [SerializeField] private float parryOp = 2f;
    [SerializeField] private float cooldown = 10f;
    [SerializeField] private string Input = "Parry";

    private Renderer[] _renderers;
    private Color[] _originalColors;

    private InputAction _parryAction;
    private bool _isParrying;

    public bool IsParrying => _isParrying;

    private float Timer;
    private float _lastParried;

    private void Start()
    {
        _parryAction = InputSystem.actions.FindAction(Input);
        _renderers = GetComponentsInChildren<Renderer>();
        _originalColors = new Color[_renderers.Length];

        for (int i = 0; i < _renderers.Length; i++)
        {
            _originalColors[i] = _renderers[i].material.color;
        }
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
            EndParry();
        }
    }

    private void TryParry()
    {
        Debug.Log("PARRY INPUT DETECTED");
        if (Time.time < _lastParried + cooldown) return;

        _lastParried = Time.time;
        _isParrying = true;
        Timer = 0f;

        SetColor(Color.blue);
        //som do parry
    }

    public void SucessfulParry(Combat enemy)
    {
        ResetColor();

        Debug.Log("PARRY SUCCESS");
        _isParrying = false;

        if (enemy == null) return;

        Vector3 direction = (enemy.transform.position - transform.position).normalized;

        if (enemy.TryGetComponent<IParryable>(out IParryable parryable))
        {
            parryable.OnParried(direction);
        }
    }

    private void EndParry()
    {
        _isParrying = false;
        ResetColor();
        
    }

    private void SetColor(Color color)
    {
        for (int i = 0; i < _renderers.Length; i++)
        {
            _renderers[i].material.color = color;
        }
    }

    private void ResetColor()
    {
        for (int i = 0; i < _renderers.Length; i++)
        {
            _renderers[i].material.color = _originalColors[i];
        }
    }
}
