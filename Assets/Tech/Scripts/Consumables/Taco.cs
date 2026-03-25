using System;
using UnityEngine;

public class Taco : MonoBehaviour
{
    [SerializeField] private float healAmount;
    void OnTriggerEnter(Collider other)
    {
        IHealable target = other.GetComponent<IHealable>();

        if (target != null && target.CanHeal())
        {
            target.Heal(healAmount);

            Destroy(gameObject, 1.0f);
        }
    }
}
