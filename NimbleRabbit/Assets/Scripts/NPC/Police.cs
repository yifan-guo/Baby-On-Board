using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Police : NPC
{
    private float ARREST_TIME = 5.0f;

    protected override void Awake() {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        Dictionary<Type, BaseState> states = new Dictionary<Type, BaseState>
        {
            {typeof(IdleState), new IdleState(this)},
            {typeof(ChaseState), new ChaseState(this)},
            {typeof(AttackState), new AttackState(this)}
        };

        stateMachine.SetStates(states);
    }

    public float StartTime {get; set;}

    public float EndTime {get; set;}

    public float TimeElapsedSinceStart
    {
        get
        {
            if (StartTime == 0)
            {
                return 0f;
            }
            return Time.time - StartTime;
        }
    }



    public override bool Chase()
    {
        Debug.Log($"Player Speed: {PlayerController.instance.GetComponent<Rigidbody>().velocity.magnitude}");
        // do not chase if player is under speed limit
        if (PlayerController.instance.GetComponent<Rigidbody>().velocity.magnitude < 0.0f) 
        {
            return false;
        }

        bool see = CanSee(
                PlayerController.instance.transform.position,
                fovMin: 0.25f,
                visionRangeMin: 10f,
                visionRangeMax: visionRange,
                lineOfSight: true);
        
        Debug.Log("Can see. going after the player");
        return see;
    }

    public override void Attack()
    {
        PlayerController player = PlayerController.instance;

        // player.rigidbody.velocity = Vector3.zero;
        // player.rigidbody.angularVelocity = Vector3.zero;
        // rigidbody.Sleep(); # https://discussions.unity.com/t/how-do-i-zero-out-the-velocity-of-an-object/2025

        // freeze the position of the player until the ARREST_TIME elapses
        player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;

        // unity is a single-threaded application
        // sleeping on the main thread will freeze the game
        // Coroutines are not threads. They run on the main thread
        // Suspend the coroutine execution for a given amount of seconds using scaled time
        StartCoroutine(WaitForArrestTime());
        // unfreeze the player
        player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
    }

    /// <summary>
    /// Coroutine to suspend player movement while ARRESTED
    /// </summary>
    IEnumerator WaitForArrestTime()
    {
        // Print the time of when the function is called
        Debug.Log($"Started Coroutine at timestamp : {Time.time}");

        // yield on a new YieldInstruction that waits for ARREST_TIME seconds
        yield return new WaitForSeconds(ARREST_TIME);

        // After we waited ARREST_TIEM seoncds print the time again
        Debug.Log($"Finished Coroutine at timestamp: {Time.time}");
    }
}