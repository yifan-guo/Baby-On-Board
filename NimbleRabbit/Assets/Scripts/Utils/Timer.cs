using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{

    [SerializeField]
    TMP_Text timerText;

    float currentTime;




    void Start() {
        currentTime = Police.ARREST_TIME;
        timerText.text = currentTime.ToString();
    }

    void OnEnable() {
        currentTime = Police.ARREST_TIME;
        timerText.text = currentTime.ToString();
    }

    void Update() {

        currentTime -= Time.deltaTime;

        if (currentTime < 0) {
            Debug.Log("timer reached zero");

            currentTime = 0;
        }

        timerText.text = "Time " + currentTime.ToString("f1");

    }

}