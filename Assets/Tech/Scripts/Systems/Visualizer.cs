using UnityEngine;
using UnityEngine.UI;

public class Visualizer : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    private PlayerStats _playerStats;

    // Only script with finds to avoid SerializeFields level references
    private void Start()
    {
        FindObjects();
        SubscribeToEvents();

        SetHealthBar();
    }

    private void FindObjects()
    {
        _playerStats = FindAnyObjectByType<PlayerStats>();
    }

    private void SubscribeToEvents()
    {
        _playerStats.OnHealthChanged += SetHealthBar;
    }

    private void SetHealthBar()
    {
        healthSlider.maxValue = _playerStats.MaxHealth;
        healthSlider.value = _playerStats.CurrentHealth;
    }
}
