using UnityEngine;
using UnityEngine.UI;

public class Visualizer : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image portrait;
    [SerializeField] private Sprite healthy, damaged;
    [SerializeField] private Slider cooldownSlider;
    private PlayerStats _playerStats;
    private PlayerParry _playerParry;

    // Only script with finds to avoid SerializeFields level references
    private void Start()
    {
        FindObjects();
        SubscribeToEvents();

        SetHealthBar();
        SetCooldownBar();
    }

    private void OnDestroy()
    {
            _playerStats.OnHealthChanged -= SetHealthBar;
            _playerParry.OnCooldownChanged -= SetCooldownBar;
    }

    private void FindObjects()
    {
        _playerStats = FindAnyObjectByType<PlayerStats>();
        _playerParry = FindAnyObjectByType<PlayerParry>();
    }

    private void SubscribeToEvents()
    {
        _playerStats.OnHealthChanged += SetHealthBar;
        _playerParry.OnCooldownChanged += SetCooldownBar;
    }

    private void SetHealthBar()
    {
        healthSlider.maxValue = _playerStats.MaxHealth;
        healthSlider.value = _playerStats.CurrentHealth;
        if (_playerStats.CurrentHealth < _playerStats.MaxHealth / 2) portrait.sprite = damaged;
        else portrait.sprite = healthy;
    }

    private void SetCooldownBar()
    {
        cooldownSlider.maxValue = 1f;
        cooldownSlider.value = _playerParry.CooldownProgress;
    }
}
