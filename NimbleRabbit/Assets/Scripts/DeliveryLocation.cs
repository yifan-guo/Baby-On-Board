using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DeliveryLocation : MonoBehaviour
{
    /// <summary>
    /// Static list of all delivery locations.
    /// </summary>
    public static List<DeliveryLocation> destinations;

    /// <summary>
    /// Clears static resources.
    /// </summary>
    public static void ClearAll()
    {
        destinations.Clear();
    }

    /// <summary>
    /// Initialization Pt I.
    /// </summary>
    private void Awake()
    {
        if (destinations == null)
        {
            destinations = new List<DeliveryLocation>();
        }

        destinations.Add(this);
    }

    /// <summary>
    /// Handles objects entering the trigger collider.
    /// </summary>
    /// <param name="other"></param>
    protected virtual void OnTriggerEnter(Collider other) 
    {
        // TODO:
        // This needs to complete the objective even if it isn't the only one.
        // Currently won't complete an objective if it doesn't beat the level.

        if (other.CompareTag("Player")) 
        {
            if (PlayerController.instance.pc.packages.Count > 0) 
            {
                // TODO() Replace with check of Level Objective completion once implemented.
                foreach (Package p in PlayerController.instance.pc.packages) 
                {
                    ((IObjective)p).CheckCompletion();
                }
            }
        }
    }
}
