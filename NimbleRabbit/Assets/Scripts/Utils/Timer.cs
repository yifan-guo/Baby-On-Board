using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{

    [SerializeField]
    TMP_Text timerText;

    [SerializeField]
    float startTime;


    float currentTime;

    bool TimerStarted = false;




    void Start() {
        currentTime = startTime;
        timerText.text = currentTime.ToString();
    }

    void OnEnable() {
        currentTime = startTime;
        timerText.text = currentTime.ToString();
    }

    void Update() {

        currentTime -= Time.deltaTime;
        Debug.Log($"currentTime: {currentTime}");

        if (currentTime < 0) {
            Debug.Log("timer reached zero");

            currentTime = 0;
        }

        timerText.text = "Time " + currentTime.ToString("f1");

    }

}