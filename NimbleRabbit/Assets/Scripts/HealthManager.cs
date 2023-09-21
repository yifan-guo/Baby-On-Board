using System;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth = 100f;
    public float damageMultiplier = 1f;
    public float healingMultiplier = 1f;
    public event Action<float, GameObject> OnDamageReceived;
    public event Action<float, GameObject> OnHealingReceived;


    private void Start()
    {
        OnDamageReceived += HandleDamage;
        OnHealingReceived += HandleHealing;

    }
    public void TakeDamage(float damageAmount, GameObject damagedObject)
    {
        OnDamageReceived?.Invoke(damageAmount, damagedObject);
    }

    public void HealDamage(float healingAmount, GameObject healedObject)
    {
        OnHealingReceived?.Invoke(healingAmount, healedObject);
    }

    public void SubscribeToDamageEvent(Action<float, GameObject> action)
    {
        OnDamageReceived += action;
    }

    public void SubscribeToHealingEvent(Action<float, GameObject> action)
    {
        OnHealingReceived += action;
    }

    private void HandleDamage(float damageAmount, GameObject damagedObject)
    {
        float newHealth = currentHealth - damageAmount;
        Debug.Log($"Ouch! {damagedObject.name} was damaged for {damageAmount}!");

        if (newHealth < 0)
        {
            currentHealth = 0;
        }
        else
        {
            currentHealth = newHealth;
        }

        Debug.Log($"{damagedObject.name} currently has {currentHealth} health points");
    }

    private void HandleHealing(float healingAmount, GameObject healedObject)
    {
        float newHealth = currentHealth + healingAmount;
        Debug.Log($"Woohoo! {healedObject.name} was healed for {healingAmount}!");

        if (newHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth = newHealth;
        }

        Debug.Log($"{healedObject.name} currently has {currentHealth} health points");
    }


    private void OnCollisionEnter(Collision collision)
    {
        DamagingHealingAttributes damagingHealingAttributes = collision.gameObject.GetComponent<DamagingHealingAttributes>();
        if (damagingHealingAttributes != null)
        {
            float damageAmount = damagingHealingAttributes.damagePerCollision * damageMultiplier;
            if (damageAmount > 0f)
            {
                TakeDamage(damageAmount, gameObject);
            }

            float healingAmount = damagingHealingAttributes.healingPerCollision * healingMultiplier;
            if (healingAmount > 0f)
            {
                HealDamage(healingAmount, gameObject);

            }
        }
    }
}
