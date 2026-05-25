using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerParry : MonoBehaviour
{
    [SerializeField] private float parryOp = 0.2f;
    [SerializeField] private string Input = "Parry";
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip parryStartSound;
    [SerializeField] private AudioClip parrySuccessSound;

    public float CooldownProgress
    {
        get
        {
            if (Time.time >= _lastParried + _cooldown)
                return 1f;

            return (Time.time - _lastParried) / _cooldown;
        }
    }    

    private Renderer[] _renderers;
    private Color[] _originalColors;

    private InputAction _parryAction;
    private bool _isParrying;

    public bool IsParrying => _isParrying;
    public float Cooldown => _cooldown;

    private float Timer;
    private float _lastParried;
    public float LastParried => _lastParried;
    private PlayerStats _stats;
    private float _cooldown;

    void Start()
    {
        _parryAction = InputSystem.actions.FindAction(Input);
        _stats = GetComponent<PlayerStats>();
        
        _renderers = GetComponentsInChildren<Renderer>();
        _originalColors = new Color[_renderers.Length];

        for (int i = 0; i < _renderers.Length; i++)
        {
            _originalColors[i] = _renderers[i].material.color;
        }
    }

    void Update()
    {
        if (_parryAction != null && _parryAction.WasPressedThisFrame())
        {
            TryParry(); 
        }

        HandleParryOpening();
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
        if (Time.time < _lastParried + _cooldown) return;

        _lastParried = Time.time;
        _isParrying = true;
        Timer = 0f;

        SetColor(Color.blue);
        //som do parry

        if (audioSource && parryStartSound)
            audioSource.PlayOneShot(parryStartSound);
    }
    private void EndParry()
    {
        _isParrying = false;
        ResetColor();   
    }

    public void SetCooldown(float value)
    {
        _cooldown = value;
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

    public void SucessfulParry(Combat enemy)
    {
        if (audioSource && parrySuccessSound)
            audioSource.PlayOneShot(parrySuccessSound);

        if (enemy == null) return;

        Vector3 direction = (enemy.transform.position - transform.position).normalized;

        if (enemy.TryGetComponent(out IParryable enemyStats))
        {
            enemyStats.ParryEffect(direction, _stats.StunDuration, _stats.KnockbackDistance);
        }
    }
}
