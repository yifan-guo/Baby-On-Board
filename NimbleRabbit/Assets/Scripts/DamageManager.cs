using System;
using UnityEngine;

public class DamageManager : MonoBehaviour
{
    public event Action<float, GameObject> OnDamageReceived;
    public event Action<float, GameObject> OnHealingReceived;


    public void  TakeDamage(float damageAmount, GameObject damagedObject)
    {
        OnDamageReceived?.Invoke(damageAmount, damagedObject);
    }

    public void HealDamage(float healingAmount, GameObject healedObject)
    {
        OnHealingReceived?.Invoke(healingAmount, healedObject);
    }
}
