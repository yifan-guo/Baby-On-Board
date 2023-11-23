using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Numerics;

public class SpeedBoostPickup : MonoBehaviour
{

    
    public float speedModifier;
    public static event Action<GameObject> OnConsumablePickedUp;
    
    public float speedBoostTime;
    public float rotationSpeed;

    private bool isBoosted;
    private float startTime;
    private float currentTime;
    private float timer;

    Rigidbody rb;
    GameObject player;

    void Awake()
    {
        isBoosted = false;
        timer = 0f;
    }


    void FixedUpdate()
    {
        RotatePickup();
        if(isBoosted)
        {
            SpeedBoost();
        }
        
    }

    private void SpeedBoost()
    {
        
        if (timer <= speedBoostTime)
        {
            rb.velocity += player.transform.forward * speedModifier * Time.deltaTime;
            timer += Time.deltaTime;
        }
        else
        {
            isBoosted = false;
            timer = 0f;
            
        }
    }

    // Give the pickup a spinning effect
    private void RotatePickup()
    {
        gameObject.transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Exit early Conditions
        if (other.gameObject.tag != "Player") return;

        // Grab reference to the player's rigid body velocity
        rb = other.GetComponent<Rigidbody>();
        player = other.gameObject;
        if (rb == null) return; 
        startTime=Time.deltaTime;
        isBoosted=true;
        transform.GetChild(0).gameObject.SetActive(false);

    }
}
