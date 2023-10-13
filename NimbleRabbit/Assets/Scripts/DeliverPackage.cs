using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliverPackage : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {

        if (other.CompareTag("Player")) {
            if (PlayerController.instance.pc.packages.Count > 0) {
                UIManager.instance.DisplayWinScreen();
                GameState.instance.TogglePause();
            }
        }
    }
}
