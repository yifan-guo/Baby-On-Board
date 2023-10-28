using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class WheelAnimator : MonoBehaviour
{
    public InputAction turn;

    [Header("Wheel Rotation")]
    public Animator flRotation;
    public Animator frRotation;
    public Animator rlRotation;
    public Animator rrRotation;

    [Header("Wheel Turning")]
    public Animator flTurn;
    public Animator frTurn;

    public float maxAnimSpeed;

    private Rigidbody carRigidbody;


    void Start()
    {
        carRigidbody = GetComponentInParent<Rigidbody>();
    }

    
    void Update()
    {

        float carSpeed = carRigidbody.velocity.magnitude;

        // Set rotation animation direction
        SetDirection();

        // Set rotation speed
        SetWheelSpeed(carSpeed);

        // Adjust animation speed as a function of the vehicle's velocity
        SetAnimationSpeed(carSpeed, maxAnimSpeed);

        // Turning animation
        SetTurnAnimation();

    }

    private void SetTurnAnimation()
    {
        float turnVal = turn.ReadValue<float>();
        flTurn.SetFloat("Turn", turnVal);
        frTurn.SetFloat("Turn", turnVal);
    }

    private void SetDirection()
    {
        Vector3 carForward = transform.forward;
        Vector3 carVelocity = carRigidbody.velocity;


        // Calculate the dot product between the forward vector and velocity vector
        float dotProduct = Vector3.Dot(carForward, carVelocity);

        if (dotProduct > 0)
        {
            // The car is moving forward
            SetWheelDirection(true);
        }
        else if (dotProduct < 0)
        {
            // The car is moving in reverse
            SetWheelDirection(false);
        }
        else
        {
            // The car is stationary
            SetWheelDirection(false);
        }
    }

    private void OnEnable()
    {
        turn.Enable();
    }

    private void OnDisable()
    {
        turn.Disable();
    }

    private void SetAnimationSpeed(float carSpeed, float maxAnimSpeed)
    {
        float normalizedSpeed = Mathf.InverseLerp(0, maxAnimSpeed, carSpeed);
        normalizedSpeed = Mathf.Clamp01(normalizedSpeed);
        float animationSpeed = normalizedSpeed * maxAnimSpeed;

        // Set the speed of the animator
        flRotation.speed = animationSpeed;
        frRotation.speed = animationSpeed;
        rlRotation.speed = animationSpeed;
        frRotation.speed = animationSpeed;
    }

    void SetWheelSpeed(float speed)
    {
        // Set the "isForward" bool parameter in each wheel's Animator Controller
        flRotation.SetFloat("Speed", speed);
        frRotation.SetFloat("Speed", speed);
        rlRotation.SetFloat("Speed", speed);
        rrRotation.SetFloat("Speed", speed);
    }

    void SetWheelDirection(bool isForward)
    {
        // Set the "isForward" bool parameter in each wheel's Animator Controller
        flRotation.SetBool("isForward", isForward);
        frRotation.SetBool("isForward", isForward);
        rlRotation.SetBool("isForward", isForward);
        rrRotation.SetBool("isForward", isForward);
    }
}
