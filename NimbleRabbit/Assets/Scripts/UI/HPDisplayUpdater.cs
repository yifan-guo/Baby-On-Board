using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

/// <summary>
/// Attach to a TextMeshProGUI object to update its text with the current and max health of a GameObject using the HealthManager.
/// </summary>
public class HPDisplayUpdater : MonoBehaviour
{
    /// <summary>
    /// The TextMeshProGUI component that will be updated based on HealthManager events.
    /// </summary>
    public TextMeshProUGUI healthText;
    public Image healthBar;

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
        healthText.text = $"{currentHealth}";
        healthBar.fillAmount = currentHealth / 100f;
    }

    void HandleHealthUpdate(
        HealthManager hp, 
        GameObject damagedObject)
    {
        // Only update the display if the event was for the player.
        // At least until we have multiple health displays and 
        // non-static HealthManager events.
        if (damagedObject.tag != "Player")
        {
            return;
        }

        SetHealthDisplay(
            hp.currentHealth, 
            hp.maxHealth);
    }

}
