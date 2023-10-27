using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliverPackage : MonoBehaviour
{
    public GameObject GameState;

    private IObjective _level;

    private void Start() {
        IObjective level = (IObjective) GameState.GetComponent(typeof(IObjective));
        if (level == null) {
            Debug.Log("Game State is missing a IObjective component");
        } else {
            _level = level;
        }
    }
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
