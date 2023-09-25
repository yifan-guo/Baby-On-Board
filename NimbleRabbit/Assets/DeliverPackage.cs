using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliverPackage : MonoBehaviour
{
    private CanvasGroup winCanvasGroup;
    // Start is called before the first frame update
    void Start()
    {
        GameObject GObj = GameObject.Find("Win UI Canvas");
        if (GObj == null) {
            Debug.LogError("No Win UI Canvas found!");
        } else {
            winCanvasGroup = GObj.GetComponent<CanvasGroup>();
            if (winCanvasGroup == null) {
                Debug.LogError("No Canvas Group found!");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other) {

        if (other.attachedRigidbody != null) {
            PackageCollector pc = other.attachedRigidbody.gameObject.GetComponent<PackageCollector>();
            if (pc != null && pc.hasPackage) { // pause the game and show the You Win panel
                winCanvasGroup.interactable = true;
                winCanvasGroup.blocksRaycasts = true;
                winCanvasGroup.alpha = 1.0f;
                Time.timeScale = 0f;
            }
        }
    }
}
