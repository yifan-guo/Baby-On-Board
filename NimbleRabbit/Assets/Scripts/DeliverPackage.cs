using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliverPackage : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {

        if (other.CompareTag("Player")) {
            if (PlayerController.instance.pc.packages.Count > 0) {
                // TODO() Replace with check of Level Objective completion once implemented.
                foreach (Package p in PlayerController.instance.pc.packages) {
                    ((IObjective)p).CheckCompletion();
                }
            }
        }
    }
}
