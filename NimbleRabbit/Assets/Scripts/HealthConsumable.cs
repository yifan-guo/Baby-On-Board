using System;
using UnityEngine;

/// <summary>
/// A health consumable is an item that never enters the players inventory.
/// It modifies the health of the GameObject it comes into contact with.
/// Then it raises an event to notify subscribers that it was consumed and it deactivates itself.
/// The health consumable must have a collider that is set to "trigger" mode.
/// </summary>
public class HealthConsumable : Collectible
{

    /// <summary>
    /// Tag to determine which other objects can trigger the consumable behavior. Defaults to "Player".
    /// </summary>
    public string targetTag = "Player";

    /// <summary>
    /// Defines damage amount taken by the GameObject that picks up the pickup.
    /// </summary>
    public float damagePerPickup = 0f;
    /// <summary>
    /// Defines healing amount taken by the GameObject that picks up the pickup.
    /// </summary>
    public float healingPerPickup = 0f;

    /// <summary>
    /// Public broadcast event to notify subscribers when this consumable is picked up.
    /// </summary>
    public static event Action<GameObject> OnConsumablePickedUp;

    /// <summary>
    /// The pickup is activated on collision, raising an event to notify subscribers, and then deactivating itself.
    /// </summary>
    /// <param name="other"></param>
    protected override void OnTriggerEnter(Collider other)
    {
        // Exit early Conditions
        if (other.gameObject.tag != targetTag)
        {
            return;
        }
        HealthManager healthManager = other.gameObject.transform.root.GetComponent<HealthManager>();
        if (healthManager == null) {
            return;
        }

        // Modify health
        if (damagePerPickup > 0){
            healthManager.TakeDamage(damagePerPickup);
        }

        if (healingPerPickup > 0){
            healthManager.HealDamage(healingPerPickup);
        }


        // Raise pickup event
        OnConsumablePickedUp?.Invoke(gameObject);

        // Deactivate the consumable so it disappears
        gameObject.SetActive(false);
    }
}
