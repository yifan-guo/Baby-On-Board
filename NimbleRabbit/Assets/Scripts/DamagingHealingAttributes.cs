using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
