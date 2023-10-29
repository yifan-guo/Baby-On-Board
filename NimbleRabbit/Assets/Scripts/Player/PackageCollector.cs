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

    // Update is called once per frame
    void Update()
    {

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
            Audio_PlayerCar playerCar = GetComponent<Audio_PlayerCar>();
            playerCar.playPackage();
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
    }

    /// <summary>
    /// Finish a package's lifecycle.
    /// </summary>
    /// <param name="pkg"></param>
    /// <param name="objectiveStatus"></param>
    public void Deliver(
        Package pkg,
        bool objectiveStatus)
    {
        IObjective pkgObjective = (IObjective) pkg;

        // Update objective
        if (objectiveStatus == true)
        {
            pkgObjective.Complete();
        }
        else
        {
            pkgObjective.Fail();
        }

        // Untrack in case we are a bandit
        Indicator.Untrack(pkg.transform.parent.gameObject);

        // Remove from play
        packages.Remove(pkg);
        pkg.gameObject.SetActive(false);
    }
}
