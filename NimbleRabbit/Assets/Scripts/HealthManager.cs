using System;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    private DamageManager damageManager;

    private void Start()
    {
        currentHealth = maxHealth;
        damageManager = GetComponent<DamageManager>();

        if (damageManager != null)
        {
            damageManager.OnDamageReceived += HandleDamage;
            damageManager.OnHealingReceived += HandleHealing;

        }
    }

    private void HandleDamage(float damageAmount, GameObject damagedObject)
    {
        currentHealth -= damageAmount;

        Debug.Log($"Ouch! {damagedObject.name} was damaged for {damageAmount}!");
        Debug.Log($"{damagedObject.name} currently has {currentHealth} health points");

        if(currentHealth <= 0)
        {
            // do something meaningful here
        }
    }

    private void HandleHealing(float healingAmount, GameObject healedObject)
    {
        currentHealth += healingAmount;

        Debug.Log($"Woohoo! {healedObject.name} was healed for {healingAmount}!");
        Debug.Log($"{healedObject.name} currently has {currentHealth} health points");

        if(currentHealth > maxHealth)
        {
            // do something meaningful here
        }
    }
}
