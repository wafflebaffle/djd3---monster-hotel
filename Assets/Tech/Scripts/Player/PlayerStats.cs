using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float maxHealth = 10.0f;
    //For Damage
    [SerializeField] private int powerLevel = 1;
    //To reduce second ability
    [SerializeField] private float cooldownReduction = 0.0f;

    private float _health;
    private PlayerMovement _playerMovement;

    //Propriedades
    public float CurrentHealth => _health;

    private void Start()
    {
        // Fazer um if para checkar se os valores 
        // do player prefs são zero. 
        // Se não, vai buscar os valores lá.

        //else
        _health = maxHealth;
        _playerMovement = GetComponent<PlayerMovement>();
    }

    public void TakeDamage(float damage)
    {
        _health -= damage;
        if(_health <= 0)
        {
            _health = 0;
            Death();
        }
    }

    public void Heal(float heal)
    {
        _health += heal;
        if(_health >= maxHealth)
        {
            _health = maxHealth;
        }
    }

    private void Death()
    {
        //Play death animation;
        //for tests purposes
        gameObject.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void IncrementHealth(float bonusHealth)
    {
        maxHealth += bonusHealth;
    }

    public void IncrementDamage(int bonusDamage)
    {
        powerLevel += bonusDamage;
    }

    public void MultiplyVelocity(float value)
    {
        _playerMovement.MultiplyVelocity(value);
    }

    public void DecrementCooldown(int timeToReduce)
    {
        cooldownReduction += timeToReduce;
    }
}
