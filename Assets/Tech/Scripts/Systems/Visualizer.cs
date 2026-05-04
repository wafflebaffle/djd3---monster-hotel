using UnityEngine;
using UnityEngine.UI;

public class Visualizer : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
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
    }

    private void SetCooldownBar()
    {
        cooldownSlider.maxValue = _playerParry.Cooldown;
        cooldownSlider.value = _playerParry.LastParried;
    }
}
