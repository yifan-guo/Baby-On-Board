using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    /// <summary>
    /// Length of car startup based on sound.
    /// Not the full length of the clip, just the length of the time it takes
    /// the engine to start in the clip.
    /// </summary>
    public const float STARTUP_TIME_S = 0.9f;

    /// <summary>
    /// Sound of car starting up.
    /// </summary>
    public AudioClip startup;

    /// <summary>
    /// Sound of car engine running.
    /// </summary>
    public AudioClip engine;

    /// <summary>
    /// Sound to play when hitting something.
    /// </summary>
    public AudioClip crash;

    /// <summary>
    /// Sound to play when picking something up.
    /// </summary>
    public AudioClip pickup;

    /// <summary>
    /// AudioSource for engine sounds.
    /// </summary>
    private AudioSource engineSource;

    /// <summary>
    /// AudioSource for one shot sounds.
    /// </summary>
    private AudioSource otherSource;

    /// <summary>
    /// Initialization Pt I.
    /// </summary>
    private void Awake()
    {
        AudioSource[] sources = GetComponents<AudioSource>();
        engineSource = sources[0];
        otherSource = sources[1];
    }

    /// <summary>
    /// Initialization Pt II.
    /// </summary>
    private void Start() 
    {
        engineSource.velocityUpdateMode = AudioVelocityUpdateMode.Dynamic;
        otherSource.velocityUpdateMode = AudioVelocityUpdateMode.Dynamic;

        engineSource.playOnAwake = false;
        engineSource.pitch = 1f;
        engineSource.loop = false;

        otherSource.playOnAwake = false;
        otherSource.pitch = 1f;
        otherSource.loop = false;

        UIManager.instance.settingsMenu.OnSoundVolumeChanged += SetSoundVolume;
    }

    /// <summary>
    /// Every frame update loop.
    /// </summary>
    private void Update()
    {
        // Wait for engine to be started
        if (PlayerController.instance.started == false)
        {
            return;
        }

        //if car is not moving, the sound pitch will stay at 20% 
        //if car is moving, the pitch will go up, but not higher than 120%
        engineSource.pitch = 0.2f;

        //get car speed
        float carSpeed = PlayerController.instance.rb.velocity.magnitude;

        if (carSpeed > 0f) 
        {
            float pitch = (0.2f + (carSpeed / 60));

            if (pitch > 1.2f)
            {
                pitch = 1.2f;
            }

            engineSource.pitch = pitch;
        }
    }

    /// <summary>
    /// Collision handler that matches sounds to tag of object.
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Package":
            case "Healing":
                PlayPickup();
                break;

            default:
                PlayCrash();
                break;
        }
    }

    /// <summary>
    /// Settings sound audio slider bar listener.
    /// </summary>
    /// <param name="value"></param>
    private void SetSoundVolume(float value)
    {
        AuditLogger.instance.ar.finalSFXVolume = value;
        engineSource.volume = value;
        otherSource.volume = value;
    }

    /// <summary>
    /// Stop sound.
    /// </summary>
    public void StopEngine()
    {
        engineSource.Stop();
    }

    /// <summary>
    /// Play startup sound.
    /// </summary>
    public void PlayStartup()
    {
        engineSource.clip = startup;
        engineSource.loop = false;
        engineSource.pitch = 1f;
        engineSource.Play();
    }

    /// <summary>
    /// Play the engine sound.
    /// </summary>
    public void PlayEngine()
    {
        engineSource.loop = true;
        engineSource.pitch = 0.2f;
        engineSource.clip = engine;
        engineSource.Play();
    }

    /// <summary>
    /// Play crash sound.
    /// </summary>
    public void PlayCrash()
    {
        otherSource.PlayOneShot(crash);
    }

    /// <summary>
    /// Play pickup sound.
    /// </summary>
    public void PlayPickup()
    {
        otherSource.PlayOneShot(pickup);
    }
}
