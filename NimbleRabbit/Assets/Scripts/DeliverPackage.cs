using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliverPackage : MonoBehaviour
{
    [SerializeField]
    private GameObject WinScreen;


    // private CanvasGroup winCanvasGroup;

    // Start is called before the first frame update
    void Start()
    {
        WinScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other) {

        if (other.CompareTag("Player")) {
            if (PlayerController.instance.packages.Count > 0) {
                WinScreen.SetActive(true);
                Time.timeScale = 0f;
            }
        }
    }
}
