using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// Static instance of PlayerController.
    /// </summary>
    public static PlayerController instance {get; private set;}

    public Rigidbody rb;
    public GameObject vehicle;

    public Transform Right;
    public Transform Left;
    public Transform Straight;

    public float forwardSpeed = 150f;
    public float backwardsSpeed = 80f;
    public float turnSpeed = 15f;

    /// <summary>
    /// Initialization Pt I.
    /// </summary>
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Initialization Pt II.
    /// </summary>
    private void Start()
    {
        instance = this;
    }

    private void Update()
    {
        print(rb.velocity);
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetKey("w"))
        {
            rb.velocity += transform.forward * forwardSpeed * Time.deltaTime;
        }

        if (Input.GetKey("s"))
        {
            rb.velocity -= transform.forward * backwardsSpeed * Time.deltaTime;
        }

        // Turning left
        if (Input.GetKey("a"))
        {
            transform.Rotate(Vector3.up, -turnSpeed * Time.deltaTime);
        }

        // Turning right
        if (Input.GetKey("d"))
        {
            transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime);
        }
    }
}
