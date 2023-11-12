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
        if (other.CompareTag("Player") == false)
        {
            return;
        }

        if (PlayerController.instance.pc.packages.Count <= 0)
        {
            return;
        }

        // Loop backwards so that we can remove packages from list as we iterate
        for (int i = PlayerController.instance.pc.packages.Count - 1;
            i >= 0;
            i--)
        {
            Package p = PlayerController.instance.pc.packages[i];

            if (p.deliveryLocation != gameObject)
            {
                continue;
            }

            p.obj.CheckCompletion();

            if (p.obj.ObjectiveStatus == IObjective.Status.Complete)
            {
                PlayerController.instance.pc.Deliver(p);
            }
        }
    }
}
