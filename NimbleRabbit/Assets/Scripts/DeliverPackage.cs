using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliverPackage : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {

        if (other.CompareTag("Player")) {
            if (PlayerController.instance.packages.Count > 0) {
                UIManager.instance.DisplayWinScreen();
                Time.timeScale = 0f;
            }
        }
    }
}
