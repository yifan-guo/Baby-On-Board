using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Police : NPC
{
    private float ARREST_TIME = 5.0f;
    
    private float SPEED_LIMIT = 15.0f;

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
            {typeof(AttackState), new AttackState(this)},
            {typeof(EngineFailureState), new EngineFailureState(this)}
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
        // unity is a single-threaded application
        // sleeping on the main thread will freeze the game
        // Coroutines are not threads. They run on the main thread
        Debug.Log("Start Couroutine");
        StartCoroutine(FreezePlayer());

        // police should rest before making another arrest
        inCooldown = true;
    }

    IEnumerator FreezePlayer() 
    {   
        Debug.Log("freeze player");
        PlayerController.instance.rb.constraints = RigidbodyConstraints.FreezePosition;

         yield return new WaitForSeconds(ARREST_TIME);

        PlayerController.instance.rb.constraints = RigidbodyConstraints.None;
    }
}