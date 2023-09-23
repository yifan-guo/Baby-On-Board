using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach to an object to trigger healing or damaging events when a collision with a object using HealthManager occurs.
/// </summary>
public class DamagingHealingAttributes : MonoBehaviour
{
    /// <summary>
    /// Defines damage amount during collision with another GameObject that uses HealthManager.
    /// </summary>
    public float damagePerCollision = 0f;
    /// <summary>
    /// Determines healing amount during collision with another GameObject that uses HealthManager.
    /// </summary>
    public float healingPerCollision = 0f;

}
