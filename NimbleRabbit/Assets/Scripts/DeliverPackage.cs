using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DeliverPackage : MonoBehaviour
{
    /// <summary>
    /// Handles objects entering the trigger collider.
    /// </summary>
    /// <param name="other"></param>
    protected virtual void OnTriggerEnter(Collider other) 
    {
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
