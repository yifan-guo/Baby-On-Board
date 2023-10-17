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
    /// Minimum force threshold to damage the player.
    /// </summary>
    public const float FORCE_MIN_THRESHOLD_TO_DMG = 10f;

    /// <summary>
    /// Maximum force that will be used to damage the player.
    /// </summary>
    public const float FORCE_MAX_THRESHOLD_TO_DMG = 40f;

    /// <summary>
    /// Percent of force that is used as damage.
    /// </summary>
    public const float FORCE_AS_DMG = 0.25f;

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

    /// <summary>
    /// Applies force to us and subsequent damage. Used for collisions with
    /// NPCs. Hitting static or non-kinematic objects like walls or debris 
    /// is covered by OnCollisionEnter().
    /// </summary>
    /// <param name="rb"></param>
    /// <param name="force"></param>
    /// <param name="mode"></param>
    public void Hit(
        Rigidbody rb,
        Vector3 force,
        ForceMode mode=ForceMode.Impulse)
    {
        // Rigidbody should be from the same object as HealthManager.
        // Might as well pass it from the caller rather than keeping another reference here.
        // If it's too gross in the future we can change it, won't be hard.
        if (rb.gameObject != gameObject)
        {
            Debug.LogError($"Rigidbody provided is for {rb.gameObject.name}, needs to be from {gameObject.name}");
            return;
        }

        rb.AddForce(
            force,
            mode);

        if (force.magnitude <= FORCE_MIN_THRESHOLD_TO_DMG) 
        {
            return;
        }

        float dmg = Mathf.Min(
            force.magnitude * FORCE_AS_DMG,
            FORCE_MAX_THRESHOLD_TO_DMG);

        TakeDamage(dmg);
    }

    public void TakeDamage(float damageAmount)
    {
        // Update internal state
        damageAmount = Mathf.Floor(damageAmount);
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
        healingAmount = Mathf.Floor(healingAmount);
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
        if (force > FORCE_MIN_THRESHOLD_TO_DMG)
        {
            // Initial force damage
            float damageAmount = Mathf.Min(
                force * FORCE_AS_DMG,
                FORCE_MAX_THRESHOLD_TO_DMG);

            // Check for scaled additional damage
            if (damagingHealingAttributes != null)
            {
                float forceScaling = 
                    (force - FORCE_MIN_THRESHOLD_TO_DMG) /
                    (FORCE_MAX_THRESHOLD_TO_DMG - FORCE_MIN_THRESHOLD_TO_DMG);

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
