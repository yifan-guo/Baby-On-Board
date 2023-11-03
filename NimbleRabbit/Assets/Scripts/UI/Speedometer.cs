using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Speedometer : MonoBehaviour
{
    private TMP_Text textMPH;
    private Image imageNeedle;

    private float carSpeed;
    private float angle;

    private float positionZero = 222.5f;
    private float degreeEachSpeed = 2f;

    private void Awake()
    {
        textMPH = transform.Find("MPH").GetComponent<TMP_Text>();
        imageNeedle = transform.Find("Needle").GetComponent<Image>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        carSpeed = PlayerController.instance.rb.velocity.magnitude;
        updateSpeedometer(carSpeed);
    }

    void updateSpeedometer(float speed)
    {
        angle = positionZero - (speed * degreeEachSpeed);
        imageNeedle.transform.eulerAngles = new Vector3(0, 0, angle);
        textMPH.text = (speed * 1.355f).ToString("f0");
    }
}
