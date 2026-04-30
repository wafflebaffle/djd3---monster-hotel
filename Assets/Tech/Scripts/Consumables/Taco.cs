using System;
using UnityEngine;

public class Taco : MonoBehaviour
{
    [SerializeField] private float healAmount;
    [SerializeField] private float destroyTimer = 1;
    void OnTriggerEnter(Collider other)
    {
        IHealable target = other.GetComponent<IHealable>();

        if (target != null && target.CanHeal())
        {
            target.Heal(healAmount);

            Destroy(gameObject, destroyTimer);
        }
    }
}
