using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

/// <summary>
/// Attach to a TextMeshProGUI object to update its text with the current and max health of a GameObject using the HealthManager.
/// </summary>
public class HPDisplayUpdater : MonoBehaviour
{
    /// <summary>
    /// The TextMeshProGUI component that will be updated based on HealthManager events.
    /// </summary>
    public TextMeshProUGUI healthDisplay;

    void OnEnable()
    {
        HealthManager.OnDamageReceived += HandleHealthUpdate;
        HealthManager.OnHealingReceived += HandleHealthUpdate;
    }


    void OnDisable()
    {
        HealthManager.OnDamageReceived -= HandleHealthUpdate;
        HealthManager.OnHealingReceived -= HandleHealthUpdate;
    }

    void SetHealthDisplay(float currentHealth, float maxHealth)
    {
        healthDisplay.text = $"HP: {currentHealth}/{maxHealth}";
    }

    void HandleHealthUpdate(float damageAmount, GameObject damagedObject)
    {
        HealthManager healthManager = damagedObject.GetComponent<HealthManager>();
        SetHealthDisplay(healthManager.currentHealth, healthManager.maxHealth);
    }

}
