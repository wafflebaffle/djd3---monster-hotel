using UnityEngine;
using UnityEngine.InputSystem;

public class Cheats : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputAction toggleCheats;
    [SerializeField] private InputAction heal;
    [SerializeField] private InputAction killAll;
    [SerializeField] private InputAction godMode;
    [SerializeField] private InputAction bigDamage;

    private bool _godMode;

    private bool _cheatsEnabled;

    private PlayerStats _player;

    void Awake()
    {
        _player = FindFirstObjectByType<PlayerStats>();
    }

    void OnEnable()
    {
        toggleCheats.Enable();
        heal.Enable();
        killAll.Enable();
        godMode.Enable();
        bigDamage.Enable();
    }

    void OnDisable()
    {
        toggleCheats.Disable();
        heal.Disable();
        killAll.Disable();
        godMode.Disable();
        bigDamage.Disable();
    }

    void Update()
    {
        if (toggleCheats.WasPressedThisFrame())
        {
            _cheatsEnabled = !_cheatsEnabled;
            Debug.Log("Cheats: " + _cheatsEnabled);
        }

        if (!_cheatsEnabled) return;
        
        if (heal.WasPressedThisFrame()) _player.Heal(999);
        if (killAll.WasPressedThisFrame()) KillAllEnemies();
        if (godMode.WasPressedThisFrame()) ToggleGodMode();
        if (bigDamage.WasPressedThisFrame()) _player.IncrementDamage(800);
    }

    private void KillAllEnemies()
    {
        EnemyStats[] enemies = FindObjectsByType<EnemyStats>(FindObjectsSortMode.None);

        foreach (EnemyStats e in enemies)
        {
            e.TakeDamage(99999, _player.GetComponent<Combat>());
        }
    }

    private void ToggleGodMode()
    {
        if (_player == null) return;

        _godMode = !_godMode;
        _player.SetGodMode(_godMode);

        Debug.Log("God Mode: " + _godMode);
    }
}
