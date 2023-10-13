using System;
using UnityEngine;

/// <summary>
/// Attach to an object to maintain state for current and max health. Also raises health events on collision with objects using DamagingHealingAttributes.
/// </summary>
public class HealthManager : MonoBehaviour
{
    /// <summary>
    /// Amount of time that the object is invulnerable after taking damage. Helps to avoid multiple damages due to stuttering / friction on collision.
    /// </summary>
    public const float PHYSICAL_DAMAGE_DEBOUNCE_S = 0.5f;

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
    /// The timestamp of the last occurrence of physical damage in seconds, e.g. damage from a physics collision with an object.
    /// </summary>
    private float timeLastPhysicalDamageSeconds;

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
        timeLastPhysicalDamageSeconds = -1f;
    }

    public void TakeDamage(float damageAmount)
    {
        // Update internal state
        float newHealth = currentHealth - damageAmount;
        // Debug.Log($"Ouch! {gameObject.name} was damaged for {damageAmount}!");

        if (newHealth < 0)
        {
            currentHealth = 0;
        }
        else
        {
            currentHealth = newHealth;
        }
        
        timeLastPhysicalDamageSeconds = Time.time;

        // Debug.Log($"{gameObject.name} currently has {currentHealth} health points");
        // Broadcast event to notify subscribers
        OnDamageReceived?.Invoke(damageAmount, gameObject);
    }

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

        timeLastPhysicalDamageSeconds = Time.time;

        // Debug.Log($"{gameObject.name} currently has {currentHealth} health points");
        // Broadcast event to notify subscribers
        OnHealingReceived?.Invoke(healingAmount, gameObject);
    }

    /// <summary>
    /// Method to tell if damage is taking place during the debounce window.
    /// </summary>
    /// <returns>Returns boolean that identifies whether the object is within the debounce window from the last time physical damage occurred.</returns>
    private bool debounceIsActive()
    {
        return Time.time < timeLastPhysicalDamageSeconds + PHYSICAL_DAMAGE_DEBOUNCE_S;
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

        float force = collision.impulse.magnitude;

        // Only damage player if the force was strong enough
        if (force > PlayerController.FORCE_MIN_THRESHOLD_TO_DMG)
        {
            // Initial force damage
            float damageAmount = Mathf.Min(
                force * PlayerController.FORCE_AS_DMG,
                PlayerController.FORCE_MAX_THRESHOLD_TO_DMG);

            // Check for scaled additional damage
            if (damagingHealingAttributes != null)
            {
                float forceScaling = 
                    (force - PlayerController.FORCE_MIN_THRESHOLD_TO_DMG) /
                    (PlayerController.FORCE_MAX_THRESHOLD_TO_DMG - PlayerController.FORCE_MIN_THRESHOLD_TO_DMG);

                damageAmount += (damagingHealingAttributes.damagePerCollision * forceScaling);
            }

            damageAmount *= damageMultiplier;

            if (damageAmount > 0f)
            {
                TakeDamage(damageAmount);
            }
        }

        if (damagingHealingAttributes != null)
        {
            float healingAmount = damagingHealingAttributes.healingPerCollision * healingMultiplier;
            if (healingAmount >= 0f)
            {
                HealDamage(healingAmount);
            }
        }
    }
}
