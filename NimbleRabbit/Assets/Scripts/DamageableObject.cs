using UnityEngine;

public class DamageableObject : MonoBehaviour
{
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
