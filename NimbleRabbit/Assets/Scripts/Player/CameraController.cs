using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float sensitivity = 2.0f; 
    public Transform target;

    // private float rotationX = 0;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        print("X = " + mouseX);
        //print( "Y = " + mouseY);

        // Rotate the player (or the object you want to rotate) horizontally


        //target.Rotate(Vector3.up * mouseX);

        // Rotate the camera vertically
        //rotationX -= mouseY;
        //rotationX = Mathf.Clamp(rotationX, -90, 90); // Limit vertical rotation to avoid flipping
        //transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
    }
}