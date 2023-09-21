using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HPDisplayUpdater : MonoBehaviour
{
    public TextMeshProUGUI healthDisplay;
    public GameObject healthfulObject;
    private HealthManager healthManager;

    void Start()
    {
        healthManager = healthfulObject.GetComponent<HealthManager>();
        healthManager.SubscribeToDamageEvent((damageAmount, damagedObject) => SetHealthDisplay());
        healthManager.SubscribeToHealingEvent((healingAmount, healedObject) => SetHealthDisplay());
        SetHealthDisplay();
    }

    void SetHealthDisplay()
    {
        healthDisplay.text = $"HP: {healthManager.currentHealth}/{healthManager.maxHealth}";
    }

}
