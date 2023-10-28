using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BanditHQ : MonoBehaviour 
{
    /// <summary>
    /// Static list of all BanditHQ locations in scene.
    /// </summary>
    public static List<BanditHQ> allHQs {get; private set;}

    /// <summary>
    /// Static map of which BanditHQ has claimed which Package.
    /// </summary>
    public static Dictionary<Package, BanditHQ> claimedPackages {get; private set;}

    /// <summary>
    /// Reset static resources.
    /// </summary>
    public static void Restart()
    {
        allHQs?.Clear();
        claimedPackages?.Clear();
    }

    /// <summary>
    /// Assign a package to a BanditHQ.
    /// </summary>
    public static void Mark(Package pkg)
    {
        // Check if package is already marked
        if (claimedPackages.ContainsKey(pkg) == true)
        {
            return;
        }

        // Select a random HQ to claim package
        int idx = Random.Range(
            0,
            allHQs.Count);

        claimedPackages.Add(
            pkg,
            allHQs[idx]);
    }

    /// <summary>
    /// Initialization Pt I.
    /// </summary>
    protected void Awake()
    {
        if (allHQs == null)
        {
            allHQs = new List<BanditHQ>();
        }

        if (claimedPackages == null)
        {
            claimedPackages = new Dictionary<Package, BanditHQ>();
        }
    }

    /// <summary>
    /// Initialization Pt II.
    /// </summary>
    protected void Start()
    {
        allHQs.Add(this);
    }

    /// <summary>
    /// Handles objects entering the trigger collider.
    /// </summary>
    /// <param name="other"></param>
    protected void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC") == false)
        {
            return;
        }

        Bandit bandit = other.GetComponent<Bandit>();
        if (bandit == null) 
        {
            return;
        }

        // Check if this bandit has a package marked by this HQ
        foreach (Package pkg in bandit.pc.packages)
        {
            if (claimedPackages.ContainsKey(pkg) == false)
            {
                continue;
            }

            // Fail player objective and finalize package steal
            if (claimedPackages[pkg] == this)
            {
                bandit.pc.Deliver(
                    pkg,
                    false);

                return;
            }
        }
    }
}