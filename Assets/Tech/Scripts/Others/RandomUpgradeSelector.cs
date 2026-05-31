using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RandomUpgradeSelector : MonoBehaviour
{
    [SerializeField] private List<Upgrade> upgrades;
    [SerializeField] private List<Button> buttons;
    [SerializeField] private RunManager runManager;
    private List<Upgrade> _usedUpgrades;
    private PlayerStats _player; 
    public void SetPlayer(PlayerStats player) => _player = player;

    private void OnEnable()
    {
        Time.timeScale = 0;
        _usedUpgrades = new();

        foreach (Button button in buttons)
        {
            Upgrade toUse = upgrades[Random.Range(0, upgrades.Count)];
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => SelecteUpgrade(toUse));
            button.GetComponentInChildren<TextMeshProUGUI>().text = toUse.Name;
            _usedUpgrades.Add(toUse);
            upgrades.Remove(toUse);
        }

        foreach (Upgrade upgrade in _usedUpgrades)
        {
            if(!upgrades.Contains(upgrade)) upgrades.Add(upgrade);
        }

        _usedUpgrades.Clear();
    }

    private void SelecteUpgrade(Upgrade upgrade)
    {
        upgrade.Effect(_player, gameObject);
        Time.timeScale = 1;
        runManager.NextLevel();
    }
}
