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


    void OnEnable()
    {
        OnDamageReceived += HandleDamage;
        OnHealingReceived += HandleHealing;
    }

    void Start()
    {
        HealDamage(0, gameObject);
    }


    void OnDisable()
    {
        OnDamageReceived -= HandleDamage;
        OnHealingReceived -= HandleHealing;
    }

    public void TakeDamage(float damageAmount, GameObject damagedObject)
    {
        OnDamageReceived?.Invoke(damageAmount, damagedObject);
    }

    public void HealDamage(float healingAmount, GameObject healedObject)
    {
        OnHealingReceived?.Invoke(healingAmount, healedObject);
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
            if (damageAmount >= 0f)
            {
                TakeDamage(damageAmount, gameObject);
            }

            float healingAmount = damagingHealingAttributes.healingPerCollision * healingMultiplier;
            if (healingAmount >= 0f)
            {
                HealDamage(healingAmount, gameObject);

            }
        }
    }
}
