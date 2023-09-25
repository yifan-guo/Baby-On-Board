using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackageCollector : MonoBehaviour
{
    /// <summary>
    /// Packages that player has collected.
    /// </summary>
    public bool hasPackage;

    // Start is called before the first frame update
    void Start()
    {
        hasPackage = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CollectPackage() {
        hasPackage = true;
    }

    public void DropPackage() {
        hasPackage = false;
    }
}
