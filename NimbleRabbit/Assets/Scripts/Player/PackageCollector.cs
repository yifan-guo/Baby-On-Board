using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackageCollector : MonoBehaviour
{
    /// <summary>
    /// Packages that player has collected.
    /// </summary>
    public List<Package> packages;

    // Start is called before the first frame update
    void Start()
    {
        packages = new List<Package>();
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
    }
}
