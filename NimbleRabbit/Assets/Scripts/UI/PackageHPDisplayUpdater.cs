using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;


/// <summary>
/// Attach this script to an empty GameObject to use as an anchor transform.
/// The list of package health displays will appear beneath the anchor transform's location.
/// </summary>
public class PackageHPDisplayUpdater : MonoBehaviour
{
    public GameObject packageHealthDisplayPrefab;

    private HealthManager hp;

    private PackageCollector pc;

    private List<GameObject> healthDisplays = new List<GameObject>();

    private const float PADDING = 10f;


    public void Link(HealthManager hp, PackageCollector pc)
    {
        this.hp = hp;
        this.pc = pc;
        SetHealthDisplay();
        this.hp.OnHealthChange += SetHealthDisplay;
        this.pc.OnInventoryChange += SetHealthDisplay;
        this.gameObject.SetActive(true);
    }

    public void UnLink()
    {
        ClearHealthDisplay();
        hp.OnHealthChange -= SetHealthDisplay;
        pc.OnInventoryChange -= SetHealthDisplay;
        hp = null;
        pc = null;
        this.gameObject.SetActive(false);
    }


    void ClearHealthDisplay()
    {
        foreach (GameObject healthDisplay in healthDisplays)
        {
            Destroy(healthDisplay);
        }
        healthDisplays.Clear();
    }

    void SetHealthDisplay()
    // We don't maintain any state here, we just reference the player and the current state of its package collector.
    // On any change in inventory or package condition, we fully destroy and recreate the package health displays.
    {
        ClearHealthDisplay();

        int i = 0;
        foreach (Package pkg in pc.packages)
        {

            GameObject packageHealthDisplay = Instantiate(packageHealthDisplayPrefab, transform);
            RectTransform packageHealthDisplayRectTransform = packageHealthDisplay.GetComponent<RectTransform>();

            Vector3 newPosition = new Vector3(
                transform.position.x,
                transform.position.y - (i * (packageHealthDisplayRectTransform.rect.height + PADDING)),
                packageHealthDisplayRectTransform.position.z
            );

            packageHealthDisplayRectTransform.position = newPosition;

            packageHealthDisplay.transform.Find("HP Numeric").GetComponent<TextMeshProUGUI>().text = $"{pkg.hp.currentHealth}";
            packageHealthDisplay.transform.Find("HP Visual").GetComponent<Image>().fillAmount = pkg.hp.currentHealth / pkg.hp.maxHealth;
            healthDisplays.Add(packageHealthDisplay);
            i++;
        }
    }
}
