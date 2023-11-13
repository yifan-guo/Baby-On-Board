using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Police : NPC
{
    public const float ARREST_TIME = 5.0f;
    
    public const float SPEED_LIMIT = 15.0f;

    private GameObject pullOverScreen;

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
        stateMachine.OnStateChanged += SetPoliceIndicator;

        pullOverScreen = UIManager.instance.pullOverScreen;

    }

    protected void SetPoliceIndicator(BaseState state)
    {
        if (PoliceIndicator.ACTIVE_STATES.Contains(state.GetType()) == true)
        {
            PoliceIndicator.Track(this);
        }
        else
        {
            PoliceIndicator.Untrack(this);
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
        
        return see;
    }

    public override void Attack()
    {
        // unity is a single-threaded application
        // sleeping on the main thread will freeze the game
        // Coroutines are not threads. They run on the main thread
        StartCoroutine(FreezePlayer());

        pullOverScreen.SetActive(true);

        // police should rest before making another arrest
        inCooldown = true;
    }

    IEnumerator FreezePlayer() 
    {   
        PlayerController.instance.rb.constraints = RigidbodyConstraints.FreezePosition;
        this.rb.constraints = RigidbodyConstraints.FreezePosition;

        yield return new WaitForSeconds(ARREST_TIME);

        pullOverScreen.SetActive(false);

        PlayerController.instance.rb.constraints = RigidbodyConstraints.None;
        this.rb.constraints = RigidbodyConstraints.None;
    }
}