using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Police : NPC
{
    private float ARREST_TIME = 5.0f;
    private float SLEEP_TIME = 10.0f;
    
    private float SPEED_LIMIT = 5.0f;

    private bool PULLED_PLAYER_OVER = false;

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
        // do not chase if player is under speed limit
        if (PULLED_PLAYER_OVER) {
            return false;
        }

        if (PlayerController.instance.rb.velocity.magnitude < SPEED_LIMIT) 
        {
            return false;
        }

        bool see = CanSee(
                PlayerController.instance.transform.position,
                fovMin: 0.25f,
                visionRangeMin: 10f,
                visionRangeMax: visionRange,
                lineOfSight: true);
        
        // Debug.Log($"Can see. going after the player: {see}");
        return see;
    }

    public override void Attack()
    {
        if (PULLED_PLAYER_OVER) {
            return;
        }
        // unity is a single-threaded application
        // sleeping on the main thread will freeze the game
        // Coroutines are not threads. They run on the main thread
        StartCoroutine(FreezePlayer(ARREST_TIME));

        // police should rest before making another arrest
        StartCoroutine(WaitForTime(this.rb, SLEEP_TIME));
    }

    IEnumerator FreezePlayer(float sleep_time) 
    {   
         PlayerController.instance.enabled = false;

         yield return new WaitForSeconds(sleep_time);

         PlayerController.instance.enabled = true;
    }

    /// <summary>
    /// Coroutine to suspend GameObject movement while ARRESTED
    /// </summary>
    IEnumerator WaitForTime(Rigidbody rb, float sleep_time)
    {
        // When police pulls player over, make the police stop driving around
        PULLED_PLAYER_OVER = true;
        this.nav.enabled = false;
        rb.constraints = RigidbodyConstraints.FreezePosition;

        // yield on a new YieldInstruction that waits for ARREST_TIME seconds
        yield return new WaitForSeconds(sleep_time);

        // After police wakes up, let it be able to arrest player and move around again
        PULLED_PLAYER_OVER = false;
        this.nav.enabled = true;
        rb.constraints = RigidbodyConstraints.None;
    }
}