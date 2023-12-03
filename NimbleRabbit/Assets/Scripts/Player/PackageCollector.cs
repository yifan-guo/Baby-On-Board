using System;
using System.Collections.Generic;
using UnityEngine;

public class PackageCollector : MonoBehaviour
{
    /// <summary>
    /// Packages that player has collected.
    /// </summary>
    public List<Package> packages;

    [Header("Defense")]

    /// <summary>
    /// Percentage of damage that is reduced for the package.
    /// </summary>
    [Range(0, 100)]
    public float damageReduction;

    public event Action OnInventoryChange;

    // Start is called before the first frame update
    void Start()
    {
        packages = new List<Package>();
        OnInventoryChange?.Invoke();
    }

    /// <summary>
    /// Add package.
    /// </summary>
    /// <param name="pkg"></param>
    public void CollectPackage(Package pkg)
    {
        if (packages.Contains(pkg))
        {
            return;
        }

        packages.Add(pkg);
        pkg.Collect(transform);

        if (gameObject.CompareTag("Player"))
        {
            PlayerController.instance.pa.PlayPickup();
        }

        OnInventoryChange?.Invoke();
    }

    /// <summary>
    /// Drop a package.
    /// </summary>
    /// <param name="pkg"></param>
    /// <param name="thief"></param>
    public void DropPackage(
        Package pkg,
        PackageCollector thief=null)
    {
        if (packages.Contains(pkg) == false)
        {
            return;
        }

        packages.Remove(pkg);

        if (thief == null)
        {
            pkg.Drop();
        }
        else
        {
            thief.CollectPackage(pkg);
        }

        OnInventoryChange?.Invoke();
        pkg.RaiseObjectiveUpdated();
    }

    /// <summary>
    /// Finish a package's lifecycle.
    /// </summary>
    /// <param name="pkg"></param>
    /// <param name="objectiveStatus"></param>
    public void Deliver(
        Package pkg,
        bool objectiveStatus = true)
    {
        // Update objective
        if (objectiveStatus == false)
        {
            IObjective pkgObjective = (IObjective) pkg;
            pkgObjective.Fail();
        }

        UIManager.instance.sm.ScorePackage(objectiveStatus ? pkg.hp.currentHealth : 0f);

        // Untrack in case we are a bandit
        Indicator.Untrack(pkg.gameObject);

        // Untrack if we delivered it
        Indicator.Untrack(pkg.deliveryLocation.gameObject);

        // Remove from play
        packages.Remove(pkg);
        pkg.gameObject.SetActive(false);

        // Update inventory
        OnInventoryChange?.Invoke();
    }
}
