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
    /// Amount of time that the object is invulnerable after taking damage. Helps to avoid multiple damages due to stuttering / friction on collision.
    /// </summary>
    public float physicalDamageDebounceSeconds = 0.5f;

    /// <summary>
    /// The timestamp of the last occurrence of physical damage in seconds, e.g. damage from a physics collision with an object.
    /// </summary>
    private float timeLastPhysicalDamageSeconds;

    /// <summary>
    /// Public broadcast event for damage taken by this object.
    /// </summary>
    public event Action OnHealthChange;

    /// <summary>
    /// Initialization Pt II.
    /// </summary>
    private void Start()
    {
        timeLastPhysicalDamageSeconds = Time.time;
    }

    /// <summary>
    /// Deals damage to the attached object.
    /// </summary>
    /// <param name="damageAmount"></param>
    public void TakeDamage(float damageAmount)
    {
        // Update internal state
        // Debug.Log($"Ouch! {gameObject.name} was damaged for {damageAmount}!");
        currentHealth = Mathf.Max(
            currentHealth - damageAmount,
            0f);

        // Debug.Log($"{gameObject.name} currently has {currentHealth} health points");
        // Broadcast event to notify subscribers
        OnHealthChange?.Invoke();
    }

    /// <summary>
    /// Heals health of the attached object.
    /// </summary>
    /// <param name="healingAmount"></param>
    public void HealDamage(float healingAmount)
    {
        // Update internal state
        float newHealth = currentHealth + healingAmount;
        // Debug.Log($"Woohoo! {gameObject.name} was healed for {healingAmount}!");

        if (newHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth = newHealth;
        }

        // Debug.Log($"{gameObject.name} currently has {currentHealth} health points");
        // Broadcast event to notify subscribers
        OnHealthChange?.Invoke();
    }

    /// <summary>
    /// Method to tell if damage is taking place during the debounce window.
    /// </summary>
    /// <returns>Returns boolean that identifies whether the object is within the debounce window from the last time physical damage occurred.</returns>
    private bool debounceIsActive()
    {
        return Time.time < timeLastPhysicalDamageSeconds + physicalDamageDebounceSeconds;
    }

    /// <summary>
    /// Method to handle damage from physical collisions.
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        // Check if we're within the debounce window to avoid stuttering / multiple damages during a single bump with a damaging object.
        if (debounceIsActive())
        {
            // Debug.Log("Debounce check prevented damage");
            return;
        }

        DamagingHealingAttributes damagingHealingAttributes = collision.gameObject.GetComponent<DamagingHealingAttributes>();
        if (damagingHealingAttributes == null)
        {
            // Debug.Log("Collision with object that does not cause damage.");
            return;
        }

        float damageAmount = damagingHealingAttributes.damagePerCollision * damageMultiplier;
        if (damageAmount >= 0f)
        {
            TakeDamage(damageAmount);

            // Transfer damage to packages if we carry them
            PackageCollector collector = gameObject.GetComponent<PackageCollector>();
            if (collector != null)
            {
                foreach (Package pkg in collector.packages)
                {
                    float reductionPercent = collector.damageReduction / 100f;
                    pkg.hp.TakeDamage(damageAmount * (1 - reductionPercent));
                }
            }

            timeLastPhysicalDamageSeconds = Time.time;
        }

        float healingAmount = damagingHealingAttributes.healingPerCollision * healingMultiplier;
        if (healingAmount >= 0f)
        {
            HealDamage(healingAmount);
            timeLastPhysicalDamageSeconds = Time.time;
        }
    }

}
