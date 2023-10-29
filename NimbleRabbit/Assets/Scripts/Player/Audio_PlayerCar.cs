using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio_PlayerCar : MonoBehaviour
{
    public AudioSource engineSound;
    public AudioSource crashSound;
    public AudioSource packageSound;  
    public AudioSource healingSound;

    public float soundPitch = 0;

    private PlayerController playerController;
    public float carSpeed;

    public float totalTime = 0;

    // Start is called before the first frame update
    void Start() 
    {
        engineSound = GetComponent<AudioSource>();
        engineSound.mute = true;
        playerController = GetComponent<PlayerController>();

    }

    // Update is called once per frame
    void Update()
    {
        //when game starts, the startup audio will play automatically
        //make the car engine audio play 0.1s later than the game start, so it won't affect the startup audio
        totalTime = totalTime + Time.deltaTime;

        if (totalTime >= 0.1) 
        {
            engineSound.mute=false;
        }

        //get car speed
        carSpeed = playerController.rb.velocity.magnitude;

        //if car is not moving, the sound pitch will stay at 20% 
        if (carSpeed == 0)
        {
            soundPitch = 0.2f;
            engineSound.pitch = soundPitch;
        }
        //if car is moving, the pitch will go up, but not higher than 120%
        else if (carSpeed > 0) 
        {
            soundPitch = (0.2f + (carSpeed / 60));

            if (soundPitch > 1.2f)
            {
                soundPitch = 1.2f;
            }
            engineSound.pitch = soundPitch;
        }

    }

    //when car hits buildings, trees or healing pickups, play a sound for crashing or healing
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Package"))
        {
            playPackage();
        }
        else if (collision.gameObject.CompareTag("Healing"))
        {
            playHealing();
        }
        else
        {
            playCrash(); 
        }
    }

    public void playCrash()
    {
        crashSound.Play();
    }

    public void playPackage()
    {
        packageSound.Play();
    }

    public void playHealing()
    {
        healingSound.Play();
    }

}
