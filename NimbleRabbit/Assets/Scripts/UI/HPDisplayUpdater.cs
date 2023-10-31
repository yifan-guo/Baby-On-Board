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
    private TextMeshProUGUI healthText;

    /// <summary>
    /// Image component for health bar.
    /// </summary>
    private Image healthBar;

    /// <summary>
    /// Reference to linked HealthManager of object whose health is displayed.
    /// </summary>
    private HealthManager hp;

    /// <summary>
    /// Initialization Pt I.
    /// </summary>
    private void Awake()
    {
        healthText = transform.Find("HP Numeric").GetComponent<TextMeshProUGUI>();
        healthBar = transform.Find("HP Visual").GetComponent<Image>();
    }

    /// <summary>
    /// Subscribe to health event and initialize.
    /// </summary>
    /// <param name="hp"></param>
    public void Link(HealthManager hp)
    {
        // Store reference and subscribe to event because it's preferable to
        // only update health when needed rather than constantly checking in
        // Update

        this.hp = hp;
        SetHealthDisplay();
        hp.OnHealthChange += SetHealthDisplay;

        this.gameObject.SetActive(true);
    }

    /// <summary>
    /// Unsubscribe from health event and disable display.
    /// </summary>
    public void Unlink()
    {
        this.gameObject.SetActive(false);

        if (hp == null)
        {
            return;
        }

        hp.OnHealthChange -= SetHealthDisplay;
        hp = null;
    }

    /// <summary>
    /// Update health text.
    /// </summary>
    public void SetHealthDisplay()
    {
        healthText.text = $"{hp.currentHealth}";
        healthBar.fillAmount = hp.currentHealth / hp.maxHealth;
    }
}
