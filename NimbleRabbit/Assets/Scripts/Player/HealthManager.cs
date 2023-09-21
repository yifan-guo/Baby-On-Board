using System;
using UnityEngine;

/// <summary>
/// Attach to an object to maintain state for current and max health. Also raises health events on collision with objects using DamagingHealingAttributes.
/// </summary>
public class HealthManager : MonoBehaviour
{
    /// <summary>
    /// The desired maximum health of the GameObject.
    /// </summary>
    public float maxHealth = 100f;

    /// <summary>
    /// The current health of the GameObject.
    /// </summary>
    public float currentHealth = 100f;

    /// <summary>
    /// Use to increase or decrease the amount of damage received. Can simulate armor or curse.
    /// </summary>
    public float damageMultiplier = 1f;

    /// <summary>
    /// Use to increase the amount of healing received. Can simulate health buff or curse.
    /// </summary>
    public float healingMultiplier = 1f;

    /// <summary>
    /// Public broadcast event for damage taken by this object.
    /// </summary>
    public static event Action<float, GameObject> OnDamageReceived;

    /// <summary>
    /// Public broadcast event for healing taken by this object.
    /// </summary>
    public static event Action<float, GameObject> OnHealingReceived;


    void Start()
    {
        // Send initial empty event to notify subscribers of starting health
        HealDamage(0);
    }


    public void TakeDamage(float damageAmount)
    {
        // Update internal state
        HandleDamage(damageAmount);
        // Broadcast event to notify subscribers
        OnDamageReceived?.Invoke(damageAmount, gameObject);
    }

    public void HealDamage(float healingAmount)
    {
        // Update internal state
        HandleHealing(healingAmount);
        // Broadcast event to notify subscribers
        OnHealingReceived?.Invoke(healingAmount, gameObject);
    }


    private void HandleDamage(float damageAmount)
    {
        float newHealth = currentHealth - damageAmount;
        Debug.Log($"Ouch! {gameObject.name} was damaged for {damageAmount}!");

        if (newHealth < 0)
        {
            currentHealth = 0;
        }
        else
        {
            currentHealth = newHealth;
        }

        Debug.Log($"{gameObject.name} currently has {currentHealth} health points");
    }

    private void HandleHealing(float healingAmount)
    {
        float newHealth = currentHealth + healingAmount;
        Debug.Log($"Woohoo! {gameObject.name} was healed for {healingAmount}!");

        if (newHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth = newHealth;
        }

        Debug.Log($"{gameObject.name} currently has {currentHealth} health points");
    }


    private void OnCollisionEnter(Collision collision)
    {
        DamagingHealingAttributes damagingHealingAttributes = collision.gameObject.GetComponent<DamagingHealingAttributes>();
        if (damagingHealingAttributes != null)
        {
            float damageAmount = damagingHealingAttributes.damagePerCollision * damageMultiplier;
            if (damageAmount >= 0f)
            {
                TakeDamage(damageAmount);
            }

            float healingAmount = damagingHealingAttributes.healingPerCollision * healingMultiplier;
            if (healingAmount >= 0f)
            {
                HealDamage(healingAmount);

            }
        }
    }
}
