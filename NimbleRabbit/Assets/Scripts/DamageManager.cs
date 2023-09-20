using System;
using UnityEngine;

public class DamageManager : MonoBehaviour
{
    /* DamageManager component:
        The DamageManager component defines events for OnDamageReceived and OnHealingReceived and provides methods to raise the events.
    */
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
