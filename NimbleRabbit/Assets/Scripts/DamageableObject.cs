using UnityEngine;

public class DamageableObject : MonoBehaviour
{
    /* DamageableObject component:
        Attach to a Game Object along with the DamageManager component to raise damage events when the Game Object
        collides with objects.

        The DamageableObject component will only raise a damage event if the colliding object has DamagingHealingAttributes attached.

        The DamageableObject component raises separate events for damage and healing to allow for divergent behavior and animations.

        No events are raised when damage and healing amounts to 0.

    */
    public float damageMultiplier = 1f;
    public float healingMultiplier = 1f;
    private DamageManager damageManager;

    private void Start()
    {
        damageManager = GetComponent<DamageManager>();
        if (damageManager == null) {
            Debug.Log($"DamageManager component not found. Make sure DamageableObject and DamageManager are attached both attached to {gameObject.name}");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        DamagingHealingAttributes damagingHealingAttributes = collision.gameObject.GetComponent<DamagingHealingAttributes>();
        if (damagingHealingAttributes != null)
        {
            float damageAmount = damagingHealingAttributes.damagePerCollision * damageMultiplier;
            if (damageAmount != 0f)
            {
                damageManager?.TakeDamage(damageAmount, gameObject);
            }

            float healingAmount = damagingHealingAttributes.healingPerCollision * healingMultiplier;
            if (healingAmount != 0f)
            {
            damageManager?.HealDamage(healingAmount, gameObject);

            }

        }

    }
}
