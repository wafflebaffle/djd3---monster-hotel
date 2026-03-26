using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class BetaDebug : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private TextMeshProUGUI debugText;

    void Start()
    {
        DisplayDebug();
        playerStats.OnHealthChanged += DisplayDebug;
    }

    private void Update()
    {
        if (InputSystem.actions.FindAction("Interact").WasPressedThisFrame())
        {
            Debug.Log("E");
            playerStats.TakeDamage(3.0f);
        }
    }

    private void DisplayDebug()
    {
        debugText.text = $"{playerStats.CurrentHealth}/{playerStats.MaxHealth} \n {playerStats.PowerLevel} \n {playerStats.CooldownReduction} \n {playerStats.Speed}";
    }
}
