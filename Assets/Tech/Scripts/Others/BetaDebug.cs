using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class BetaDebug : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private EnemyStats enemyStats;
    [SerializeField] private TextMeshProUGUI debugText;

    void Start()
    {
        DisplayDebug();
        playerStats.OnHealthChanged += DisplayDebug;
        playerStats.OnBuff += DisplayDebug;
    }

    private void DisplayDebug()
    {
        debugText.text = $"{playerStats.CurrentHealth}/{playerStats.MaxHealth} \n {playerStats.AttackDamage} \n {playerStats.CooldownReduction} \n {playerStats.Speed} \n {"Enemy"} \n {enemyStats.CurrentHealth}/{enemyStats.MaxHealth}";
    }
}
