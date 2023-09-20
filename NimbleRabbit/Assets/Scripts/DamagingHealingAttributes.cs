using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagingHealingAttributes : MonoBehaviour
{
    /* DamagingHealingAttributes component:
        The DamagingHealingAttributes component specifies what amount of health or damage should be passed to a damageable object
        on contact with the current object.

        Both healing and damage are included in the same component to allow easy adding of one component to manage either behavior.
    */
    public float damagePerCollision = 0f;
    public float healingPerCollision = 0f;
}
