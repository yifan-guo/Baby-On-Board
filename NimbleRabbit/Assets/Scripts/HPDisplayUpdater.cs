using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class HPDisplayUpdater : MonoBehaviour
{
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
