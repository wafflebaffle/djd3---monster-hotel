using TMPro;
using UnityEngine;

public class BetaDebug : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private TextMeshProUGUI debugText;

    private void Update()
    {
        debugText.text = $"{playerStats.CurrentHealth}/{playerStats.MaxHealth} \n {playerStats.PowerLevel} \n {playerStats.CooldownReduction} \n {playerStats.Speed}";
    }
}
